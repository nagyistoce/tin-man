﻿/*
 * Created by Drew, 04/06/2010 02:58.
 */
using System;
using System.Text;

namespace Drew.RoboCup
{
    // TODO implement MoveUniversalJointAction, even though it's not used by Nao
    
    public interface IAction {
        void AppendCommand(StringBuilder s);
    }
    
    public sealed class MoveHingeJointAction : IAction {
        private readonly string _effectorLabel;
        // TODO work out what the _move represents. Docs say 'change in angle of the joint', some code says 'angular velocity in radians per second'
        private readonly double _move;
        public MoveHingeJointAction(string effectorLabel, double move) {
            _effectorLabel = effectorLabel;
            _move = move;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("({0} {1:0.######})", _effectorLabel, _move);
        }
    }
    
    /// <summary>
    /// Allows a player to position itself on the field before the game starts.
    /// </summary>
    public sealed class BeamAction : IAction {
        private readonly double _x;
        private readonly double _y;
        private readonly double _rotation;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rotation">Defines the rotation angle of the player. Zero degrees points to positive x axis (to
        /// the right of the field), 90 degrees to positive y axis (to the top of the field).</param>
        public BeamAction(double x, double y, double rotation) {
            _x = x;
            _y = y;
            _rotation = rotation;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(beam {0:0.####} {1:0.####} {2:0.####})", _x, _y, _rotation);
        }
    }
    
    public sealed class SceneSpecificationAction : IAction {
        private readonly string _rsgPath;
        public SceneSpecificationAction(string rsgPath) {
            _rsgPath = rsgPath;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(scene {0})", _rsgPath);
        }
    }
    
    /// <summary>
    /// This action must be sent once for player after <see cref="SceneSpecificationAction"/>.
    /// It registers this agent as a member of the specified team with the specified uniform number.
    /// All players of a team must use the same team name and different numbers. Sending a uniform number
    /// of 0 assigns the next free number automatically.
    /// Note that the side on which a team starts to play depends on which team connected first.
    /// </summary>
    public sealed class InitialisePlayerAction : IAction {
        private readonly int _uniformNumber;
        private readonly string _teamName;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// If an agent sends 0 as uniformNumber, the number is assigned automatically by the server to the next free number.
        /// </remarks>
        /// <param name="uniformNumber"></param>
        /// <param name="teamName"></param>
        public InitialisePlayerAction(int uniformNumber, string teamName) {
            _uniformNumber = uniformNumber;
            _teamName = teamName;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(init (unum {0}) (teamname {1}))", _uniformNumber, _teamName);
        }
    }
    
    /// <summary>
    /// The say effector permits communication among agents by broadcasting messages.
    /// </summary>
    public sealed class SayAction : IAction {
        /// <summary>
        /// Message may consist of 20 characters, which may be taken from the ASCII printing character
        /// subset [0x20; 0x7E] except the white space character ' ' and the normal brackets '(' and ')'.
        /// </summary>
        public static bool IsValidMessage(string message) {
            if (message==null)
                return false;
            if (message.Length==0 || message.Length>20)
                return false;
            if (message.IndexOfAny(new[] {' ', '(', ')'})!=-1)
                return false;
            foreach (var c in message) {
                if (c < 0x20 || c > 0x7E)
                    return false;
            }
            return true;
        }
        private readonly string _messageToSay;
        public SayAction(string messageToSay) {
            if (!IsValidMessage(messageToSay))
                throw new ArgumentException("Message is invalid.", "messageToSay");
            _messageToSay = messageToSay;
        }
        
        public void AppendCommand(StringBuilder s) {
            s.AppendFormat("(say {0})", _messageToSay);
        }
    }
}