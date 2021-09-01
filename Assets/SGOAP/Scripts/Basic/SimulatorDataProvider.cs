using UnityEngine;

namespace SGoap
{
    /// <summary>
    /// A data provider for the GOAP Agent Debugger simulation.
    /// Think of this as Unit Test Data
    /// This is necessary if you are using a dynamic AchievableEvaluator or CostEvaluator and the code requires data at runtime.
    /// </summary>
    public abstract class SimulatorDataProvider : MonoBehaviour
    {
        /// <summary>
        /// Set the data for anything that is needed for the simulator to work with your agent.
        /// </summary>
        public abstract void Setup();

        /// <summary>
        /// Remove any data in setup that needs to be cleaned up.
        /// </summary>
        public abstract void Clean();
    }
}