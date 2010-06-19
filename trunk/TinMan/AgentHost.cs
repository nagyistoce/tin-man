﻿#region License
/* 
 * This file is part of TinMan.
 *
 * TinMan is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TinMan is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with TinMan.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

// Copyright Drew Noakes, http://drewnoakes.com
// Created 06/05/2010 14:07

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using TinMan.PerceptorParsing;

namespace TinMan {
    /// <summary>
    /// Hosts an implementation of <see cref="IAgent"/>, handling all communication with the server and
    /// managing the lifecycle of the TinMan framework with respect to the simulation.
    /// </summary>
    public sealed class AgentHost {
        public const int DefaultTcpPort = 3100;
        public static readonly TimeSpan CyclePeriod = TimeSpan.FromMilliseconds(20);
        
        private static readonly Log _log = Log.Create();
        
        /// <summary>
        /// Creates a new client.  <see cref="HostName"/> is set to <tt>localhost</tt> and
        /// <see cref="TeamName"/> to <tt>TinManBots</tt>.  Change these explicitly after
        /// construction, but before calling <see cref="Run"/>.
        /// </summary>
        public AgentHost() {
            HostName = "localhost";
            PortNumber = DefaultTcpPort;
            TeamName = "TinManBots";
            UniformNumber = 0;
        }
        
        /// <summary>
        /// Gets and sets the name of the team to which this agent belongs.  Once the client is
        /// running, this value should no longer be changed.
        /// </summary>
        public string TeamName { get; set; }
        
        /// <summary>
        /// Gets and sets the desired uniform number for this player.  A value of zero tells the server to
        /// assign the next available number automatically.  This is the default.
        /// </summary>
        int UniformNumber { get; set; }

        /// <summary>
        /// The name of the host running the server to connect to.  By default this is <tt>localhost</tt>.
        /// </summary>
        public string HostName { get; set; }
        
        /// <summary>
        /// The TCP port number that the server is listening on.  By default this is 3100.
        /// </summary>
        public int PortNumber { get; set; }
        
        /// <summary>
        /// Gets the simulation context used by this host.  The context provides appropriately
        /// scoped access to resources provided by the TinMan framework for use by an agent.
        /// </summary>
        public ISimulationContext Context { get; private set; }
        
        private bool _stopRequested = false;
        
        /// <summary>
        /// Connects to the RoboCup3D server and runs a simulation with <paramref name="agent"/>.
        /// This call blocks until either <see cref="Stop"/> is called, or <see cref="IAgent.IsAlive"/>
        /// becomes false (usually because an agent calls <see cref="AgentBase{TBody}.StopSimulation"/>.
        /// </summary>
        /// <param name="agent"></param>
        public void Run(IAgent agent) {
            _log.Info("Connecting via TCP to {0}:{1}", HostName, PortNumber);

            // Try to make a TCP connection.
            TcpClient client;
            try {
                client = new TcpClient(HostName, PortNumber);
            } catch (SocketException ex) {
                _log.Error(ex, "Unable to connect to {0}:{1}.  Exiting.", HostName, PortNumber);
                throw;
            }

            _log.Info("Connected.");

            using (client)
                using (NetworkStream stream = client.GetStream()) {
                _log.Info("Sending initialisation messages");
                
                // Initialise with server.  We must first send the scene command, to specify which robot we'll be using.
                // NOTE We read between sends, even though no reponse will be received.  If we don't then we appear in middle, white.
                // TODO maybe just a pause is enough (rather than a read)
                // TODO at startup, don't log warnings about listen timeouts until after the first message has been received from the server
                SendCommands(stream, new [] { new SceneSpecificationCommand(agent.Body.RsgPath) });
                ReadResponseString(stream);
                // Specify which player on which team.
                SendCommands(stream, new [] { new InitialisePlayerCommand(UniformNumber, TeamName) });
                ReadResponseString(stream);
                
                var context = new SimulationContext(this);
                Context = context;
                
                var commands = new List<IEffectorCommand>();
                while (!_stopRequested) {
                    commands.Clear();
                    string data = ReadResponseString(stream);
                    if (data!=null) {
                        // Parse message
                        var parser = new Parser(new Scanner(new StringBuffer(data)));
                        parser.Parse();
                        if (parser.errors.HasError)
                            _log.Error("PARSE ERROR: {0}\nDATA: {1}", parser.errors.ErrorMessages, data);
                        
                        // Update the body's hinges with current angular positions
                        foreach (var hinge in agent.Body.AllHinges) {
                            Angle angle;
                            if (parser.State.TryGetHingeAngle(hinge, out angle))
                                hinge.Angle = angle;
                        }
                        
                        // Let the agent perform its magic
                        agent.Think(Context, parser.State);
                        
                        // Visit all hinges again to compute any control functions
                        foreach (var hinge in agent.Body.AllHinges)
                            hinge.ComputeControlFunction(context);
                        
                        // Collate list of commands to send
                        context.FlushCommands(commands);
                        foreach (var hinge in agent.Body.AllHinges) {
                            if (hinge.IsDesiredSpeedChanged)
                                commands.Add(hinge.GetCommand());
                        }
                    }
                    
                    if (commands.Count!=0)
                        SendCommands(stream, commands);
                }
                
                if (agent is IDisposable)
                    ((IDisposable)agent).Dispose();
            }
        }
        
        public void Stop() {
            _stopRequested = true;
        }
        
        private static string ReadResponseString(NetworkStream stream) {
            // It seems like a good idea to pass the stream to Coco/R rather than loading the whole
            // string into memory first, however because Coco/R requires the ability to seek within
            // the stream, it would internally load the stream into a buffer anyway.  To avoid this
            // memory churn, Coco/R should be replaced.
            int sleepCount = 0;
            while (!stream.DataAvailable) {
                if (++sleepCount > 100) {
                    _log.Warn("No response received within limit.");
                    return null;
                }
                Thread.Sleep(5);
            }
            
            int length = ReadInt32(stream);
            var bytes = new byte[length];
            
            int totalBytesRead = 0;
            do {
                totalBytesRead += stream.Read(bytes, totalBytesRead, length - totalBytesRead);
            } while (totalBytesRead < length);
            
            return Encoding.ASCII.GetString(bytes);
        }

        private static int ReadInt32(NetworkStream stream) {
            var lengthBytes = new byte[4];
            stream.Read(lengthBytes, 0, 4);
            int length = (lengthBytes[0] << 24) | (lengthBytes[1] << 16) | (lengthBytes[2] << 8) | (lengthBytes[3]);
            return length;
        }
        
        private static void SendCommands(NetworkStream stream, IEnumerable<IEffectorCommand> commands) {
            string commandStr = ConcatCommandStrings(commands);
            // Server uses 1-byte-per-character ASCII
            byte[] bytes = Encoding.ASCII.GetBytes(commandStr);
            uint num = (uint)bytes.Length;
            // Big endian - MSB first
            var numBytes = new byte[4];
            numBytes[3] = (byte)(num & 0xff);
            numBytes[2] = (byte)(num >> 8 & 0xff);
            numBytes[1] = (byte)(num >> 16 & 0xff);
            numBytes[0] = (byte)(num >> 24 & 0xff);
            // Prefix with the length of the message
            stream.Write(numBytes, 0, numBytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string ConcatCommandStrings(IEnumerable<IEffectorCommand> commands) {
            var sb = new StringBuilder();
            foreach (var command in commands)
                command.AppendSExpression(sb);
            return sb.ToString();
        }
    }
}