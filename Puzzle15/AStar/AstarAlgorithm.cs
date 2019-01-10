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

            if (currentState.IsFinalState()) {
                return GetFinalPath(currentState);
            }


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
                        openState = openedQueue.Find(nextState, out int openStateIndex);

                        if (openState.IsCostlierThan(nextState)) {
                            openedQueue.Remove(openStateIndex);
                            openedQueue.Enqueue(nextState);
                        }
                    } else {
                        string stateCode = nextState.GetStateCode();

                        if (closedQueue.TryGetValue(stateCode, out closedState)) {
                            if (closedState.IsCostlierThan(nextState)) {
                                closedQueue.Remove(stateCode);
                                closedQueue[stateCode] = nextState;
                            }
                        }
                    }

                    if (openState == null && closedState == null) {
                        openedQueue.Enqueue(nextState);
                        openStates.Add(nextState.GetStateCode());
                    }
                }

                closedQueue[currentState.GetStateCode()] = currentState;
            }
        }
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
