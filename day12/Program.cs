using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace day12
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get file
            var projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            var filePath = Path.Combine(projectDirectory, "data", "data.txt");
            using StreamReader streamReader = new StreamReader(filePath);

            var instructions = new List<string>();
            while (!streamReader.EndOfStream)
            {
                instructions.Add(streamReader.ReadLine());
            }

            Console.WriteLine($"Solution problem 1: {GetEndPositionShip(instructions, false)}"); // Problem 1
            Console.WriteLine($"Solution problem 2: {GetEndPositionShip(instructions, true)}"); // Problem 2
        }

        static int GetEndPositionShip(List<string> instructions, bool moveWaypoint)
        {
            var direction = Direction.E;            // Start off facing east
            var waypointPos = new int[] { 10, 1 };  // East/West, North/South
            var shipPos = new int[] { 0, 0 };       // East/west, North/south

            foreach (var instruction in instructions)
            {
                var command = instruction[0].ToString();
                var steps = int.Parse(instruction[1..]);

                // Move object NESW.
                if (IsCompassMovement(command))
                {
                    if (!moveWaypoint)
                    {
                        shipPos = CompassMoveObject(command, shipPos, steps);
                    }
                    else
                    {
                        waypointPos = CompassMoveObject(command, waypointPos, steps);
                    }
                }
                // Move object forward.
                else if (command.Equals("F"))
                {
                    if (!moveWaypoint)
                    {
                        shipPos = CompassMoveObject(direction.ToString(), shipPos, steps);
                    }
                    else
                    {
                        var waypointDirections = GetDirectionFromShip(shipPos, waypointPos);
                        var stepsX = Math.Abs(steps * (waypointPos[0] - shipPos[0]));
                        var stepsY = Math.Abs(steps * (waypointPos[1] - shipPos[1]));

                        shipPos = CompassMoveObject(waypointDirections, shipPos, new int[] { stepsX, stepsY });
                        waypointPos = CompassMoveObject(waypointDirections, waypointPos, new int[] { stepsX, stepsY });
                    }
                }
                // Get new object direction.
                // In the case of 'moveWaypoint', also move the waypoint.
                else
                {
                    if (!moveWaypoint)
                    {
                        direction = GetNewDirection(direction, command, steps);
                    }
                    else
                    {
                        var waypointDirections = GetDirectionFromShip(shipPos, waypointPos);
                        var moveDirections = GetNewDirections(waypointDirections, command, steps);

                        var curStepsX = Math.Abs(waypointPos[0] - shipPos[0]);
                        var curStepsY = Math.Abs(waypointPos[1] - shipPos[1]);

                        var newStepsX = IsHorizontalDirection(moveDirections[0]) ? curStepsX : curStepsY;
                        var newStepsY = newStepsX == curStepsX ? curStepsY : curStepsX;

                        waypointPos = CompassMoveObject(moveDirections, shipPos, new int[] { newStepsX, newStepsY });
                    }
                }
            }

            return Math.Abs(shipPos[0]) + Math.Abs(shipPos[1]);
        }

        // Movement N, E, S, or W.
        static bool IsCompassMovement(string command)
        {
            return
                command.Equals("N") ||
                command.Equals("E") ||
                command.Equals("S") ||
                command.Equals("W");
        }

        // [X, Y] position after N, E, S, W movement.
        static int[] CompassMoveObject(string direction, int[] curObjectPosition, int steps)
        {
            var newX = direction.Equals("W") ? curObjectPosition[0] - steps :
                direction.Equals("E") ? curObjectPosition[0] + steps : curObjectPosition[0];

            var newY = direction.Equals("S") ? curObjectPosition[1] - steps :
                direction.Equals("N") ? curObjectPosition[1] + steps : curObjectPosition[1];

            return new int[] { newX, newY };
        }

        // [X, Y] position after N, E, S, W movement.
        static int[] CompassMoveObject(Direction[] directions, int[] curObjectPosition, int[] steps)
        {
            var newX = 0;
            var newY = 0;

            foreach (var direction in directions)
            {
                newX = direction.Equals(Direction.W) ? curObjectPosition[0] - steps[0] :
                    direction.Equals(Direction.E) ? curObjectPosition[0] + steps[0] : newX;

                newY = direction.Equals(Direction.S) ? curObjectPosition[1] - steps[1] :
                    direction.Equals(Direction.N) ? curObjectPosition[1] + steps[1] : newY;
            }

            return new int[] { newX, newY };
        }

        static Direction GetNewDirection(Direction currentDirection, string command, int degrees)
        {
            var number = degrees == 90 ? 1 : degrees == 180 ? 2 : 3;
            var multiplier = command.Equals("L") ? -1 : 1;

            var turn = number * multiplier; // Flip if necessary
            var sum = (int)currentDirection + turn;

            // Include wrapping
            var newDirection = sum < (int)Direction.N ? ((int)Direction.W + 1) - Math.Abs(sum) :
                sum > (int)Direction.W ? ((int)Direction.N - 1) + (sum - (int)Direction.W) : sum;

            return (Direction)Enum.Parse(typeof(Direction), newDirection.ToString());
        }

        // Directions can either be in the format [HOR, VER] as in [VER, HOR]
        static Direction[] GetNewDirections(Direction[] currentDirections, string command, int degrees)
        {
            var directions = new List<Direction>();
            var steps = (degrees / 90) * (command.Equals("L") ? -1 : 1); // Clockwise / Counter-clockwise.

            foreach (var currentDirection in currentDirections)
            {
                var sum = (int)currentDirection + steps;

                // Include wrapping
                var newDirection = sum < (int)Direction.N ? ((int)Direction.W + 1) - Math.Abs(sum) :
                    sum > (int)Direction.W ? ((int)Direction.N - 1) + (sum - (int)Direction.W) : sum;

                directions.Add((Direction)Enum.Parse(typeof(Direction), newDirection.ToString()));
            }

            return directions.ToArray();
        }

        static Direction[] GetDirectionFromShip(int[] shipPos, int[] waypointPos)
        {
            var dirX = waypointPos[0] < shipPos[0] ? Direction.W : Direction.E;
            var dirY = waypointPos[1] < shipPos[1] ? Direction.S : Direction.N;

            return new Direction[] { dirX, dirY };
        }

        static bool IsHorizontalDirection(Direction direction)
        {
            return direction.Equals(Direction.E) || direction.Equals(Direction.W);
        }

        enum Direction
        {
            N = 0,
            E = 1,
            S = 2,
            W = 3
        }
    }
}
