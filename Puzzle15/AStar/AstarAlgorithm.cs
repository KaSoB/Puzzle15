using System.Collections.Generic;

public class AStarAlgorithm {
    public Stack<State> Run(int[] nodes, HeuristicMethod heuristic) {
        List<State> nextStates = new List<State>();
        HashSet<string> openStates = new HashSet<string>();
        MinPriorityQueue<State> openedQueue = new MinPriorityQueue<State>(nodes.Length);
        Dictionary<string, State> closedQueue = new Dictionary<string, State>();

        State state = new State(parent: null, nodes: nodes, heuristic: heuristic);
        openedQueue.Enqueue(state);
        openStates.Add(state.GetStateCode());


        while (!openedQueue.IsEmpty()) {
            State currentState = openedQueue.Dequeue();
            openStates.Remove(currentState.GetStateCode());

            // Is this final state
            if (currentState.IsFinalState()) {
                return GetFinalPath(currentState);
            }

            // Look into next state
            currentState.GetNextStates(ref nextStates);

            if (nextStates.Count > 0) {
                State closedState;
                State openState;
                State nextState;

                for (int i = 0 ; i < nextStates.Count ; i++) {
                    closedState = null;
                    openState = null;
                    nextState = nextStates[i];

                    if (openStates.Contains(nextState.GetStateCode())) {
                        // We already have same state in the open queue. 
                        openState = openedQueue.Find(nextState, out int openStateIndex);

                        if (openState.IsCostlierThan(nextState)) {
                            // We have found a better way to reach at this state. Discard the costlier one
                            openedQueue.Remove(openStateIndex);
                            openedQueue.Enqueue(nextState);
                        }
                    } else {
                        // Check if state is in closed queue
                        string stateCode = nextState.GetStateCode();

                        if (closedQueue.TryGetValue(stateCode, out closedState)) {
                            // We have found a better way to reach at this state. Discard the costlier one
                            if (closedState.IsCostlierThan(nextState)) {
                                closedQueue.Remove(stateCode);
                                closedQueue[stateCode] = nextState;
                            }
                        }
                    }

                    // Either this is a new state, or better than previous one.
                    if (openState == null && closedState == null) {
                        openedQueue.Enqueue(nextState);
                        openStates.Add(nextState.GetStateCode());
                    }
                }

                closedQueue[currentState.GetStateCode()] = currentState;
            }
        }
        // no result
        return null;
    }


    private Stack<State> GetFinalPath(State state) {
        Stack<State> path = new Stack<State>();
        while (state != null) {
            path.Push(state);
            state = state.GetParent();
        }
        return path;
    }

}
