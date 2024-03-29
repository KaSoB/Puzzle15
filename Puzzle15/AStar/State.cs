﻿using System;
using System.Collections.Generic;

public class State : IComparable {
    private int[] Nodes;
    private int SpaceIndex;
    private string StateCode;
    private int CostF;
    private int CostH;
    private int CostG;
    private HeuristicMethod Heuristic;
    private State Parent;

    public State(State parent, int[] nodes, HeuristicMethod heuristic) {
        Nodes = nodes;
        Parent = parent;
        Heuristic = heuristic;
        CalculateCost();
        StateCode = GenerateStateCode();
    }

    private State(State parent, int[] nodes) {
        Nodes = nodes;
        Parent = parent;
        Heuristic = parent.Heuristic;
        CalculateCost();
        StateCode = GenerateStateCode();
    }
    public bool IsCostlierThan(State thatState) {
        return CostG > thatState.CostG;
    }

    public string GetStateCode() {
        return StateCode;
    }

    private void CalculateCost() {
        if (Parent == null) {
            // We are at the first state - we assume we have been asked to be at this state, so no cost.
            CostG = 0;
        } else {
            // Here, state transition cost is 1 unit. Since transition from one state to another is by moving he tile one step.
            CostG = Parent.CostG + 1;
        }

        // Heuristic cost
        CostH = GetHeuristicCost();

        CostF = CostH + CostG;
    }

    private int GetHeuristicCost() {
        return (Heuristic == HeuristicMethod.ManhattanDistance) ? GetManhattanDistanceCost() : GetMisplacedTilesCost();
    }

    private int GetMisplacedTilesCost() {
        int heuristicCost = 0;

        for (int i = 0 ; i < Nodes.Length ; i++) {
            int value = Nodes[i] - 1;

            // Space tile's value is -1
            if (value == -2) {
                value = Nodes.Length - 1;
                SpaceIndex = i;
            }

            if (value != i) {
                heuristicCost++;
            }
        }

        return heuristicCost;
    }

    private int GetManhattanDistanceCost() {
        int heuristicCost = 0;
        int gridX = (int) Math.Sqrt(Nodes.Length);
        int idealX;
        int idealY;
        int currentX;
        int currentY;
        int value;

        for (int i = 0 ; i < Nodes.Length ; i++) {
            value = Nodes[i] - 1;
            if (value == -2) {
                value = Nodes.Length - 1;
                SpaceIndex = i;
            }

            if (value != i) {
                // Misplaced tile
                idealX = value % gridX;
                idealY = value / gridX;

                currentX = i % gridX;
                currentY = i / gridX;

                heuristicCost += (Math.Abs(idealY - currentY) + Math.Abs(idealX - currentX));
            }
        }

        return heuristicCost;
    }

    private string GenerateStateCode() {
        return string.Join(" ", Nodes);
    }

    public int[] GetState() {
        int[] state = new int[Nodes.Length];
        Array.Copy(Nodes, state, Nodes.Length);

        return state;
    }

    public bool IsFinalState() {
        // If all tiles are at correct position, we are into final state.
        return CostH == 0;
    }

    public State GetParent() {
        return Parent;
    }

    public List<State> GetNextStates(ref List<State> nextStates) {
        nextStates.Clear();
        State state;

        foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
            state = GetNextState(direction);

            if (state != null) {
                nextStates.Add(state);
            }
        }

        return nextStates;
    }

    private State GetNextState(Direction direction) {

        if (CanMove(direction, out int position)) {
            int[] nodes = new int[Nodes.Length];
            Array.Copy(Nodes, nodes, Nodes.Length);

            // Get new state nodes
            Swap(nodes, SpaceIndex, position);

            return new State(this, nodes);
        }

        return null;
    }

    private void Swap(int[] nodes, int i, int j) {
        int t = nodes[i];
        nodes[i] = nodes[j];
        nodes[j] = t;
    }

    private bool CanMove(Direction direction, out int newPosition) {
        int newX = -1;
        int newY = -1;
        int gridX = (int) Math.Sqrt(Nodes.Length);
        int currentX = SpaceIndex % gridX;
        int currentY = SpaceIndex / gridX;
        newPosition = -1;

        switch (direction) {
            case Direction.Up: {
                    // Can not move up if we are at the top
                    if (currentY != 0) {
                        newX = currentX;
                        newY = currentY - 1;
                    }
                }
                break;

            case Direction.Down: {
                    // Can not move down if we are the lowest level
                    if (currentY < (gridX - 1)) {
                        newX = currentX;
                        newY = currentY + 1;
                    }
                }
                break;

            case Direction.Left: {
                    // Can not move left if we are at the left most position
                    if (currentX != 0) {
                        newX = currentX - 1;
                        newY = currentY;
                    }
                }
                break;

            case Direction.Right: {
                    // Can not move right if we are at the right most position
                    if (currentX < (gridX - 1)) {
                        newX = currentX + 1;
                        newY = currentY;
                    }
                }
                break;
        }

        if (newX != -1 && newY != -1) {
            newPosition = newY * gridX + newX;
        }

        return newPosition != -1;
    }

    public override int GetHashCode() {
        return StateCode.GetHashCode();
    }
    public override bool Equals(object obj) {
        return StateCode.Equals((obj as State).StateCode);
    }
    public int CompareTo(object obj) {
        return CostF.CompareTo((obj as State).CostF);
    }
    public override string ToString() {
        return $"State: {StateCode}, g: {CostG}, h: {CostH}, f: {CostF}";
    }
}
