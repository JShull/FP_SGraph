namespace FuzzPhyte.SGraph.Samples
{
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    /// <summary>
    /// A Sample Gameobject Node
    /// </summary>
    public class NodeMBEx : UNodeMB<TransitionD,RequirementD>
    {
        public SGNode SharpData;
        public NodeDataSOBEx NodeDataGen;
        public List<NodeMBEx> ConnectedNodes = new List<NodeMBEx>();
        public List<TransitionD> EventsByTransitions = new List<TransitionD>();

        [Header("Backup Data")]
        public TextAsset JSONData;

        public override NodeSB<TransitionD, RequirementD> NodeSharp { get { return SharpData; } set { SharpData = (SGNode)value; } }
        public override NodeSOB<TransitionD, RequirementD> NodeDataTemplate { get => NodeDataGen; set { NodeDataGen = (NodeDataSOBEx)value; } }
        public override List<UNodeMB<TransitionD,RequirementD>> UnityOutConnections { get=>ConnectedNodes.Cast<UNodeMB<TransitionD,RequirementD>>().ToList(); set { ConnectedNodes = value.Cast<NodeMBEx>().ToList(); } }

        public override List<TransitionD> EventsByType { get => EventsByTransitions; set {EventsByTransitions=value;} }

        /// <summary>
        // Probably called from the Manager Class
        /// </summary>
        public SGNode SetupNodeData(List<NodeMBEx> allNodesInScene)
        {
            var reqList = NodeDataTemplate.Requirements;
            List<TransitionD> transList = new List<TransitionD>();
            for(int i=0;i<EventsByType.Count;i++)
            {
                var newTransition = new TransitionD(){
                    Transition = EventsByType[i].Transition,
                    OutcomeStatus = EventsByType[i].OutcomeStatus,
                    UnityActionEvents = EventsByType[i].UnityActionEvents
                };
                transList.Add(newTransition);
            }
            //this.BuildRuntimeNode(reqList, transList);
            //new runtime data node
            if(SharpData==null)
            {
                SharpData = new SGNode(this.gameObject.GetInstanceID().ToString());
            }
            
            //starting state from TemplateData
            //var curData = NodeDataTemplate as SOSGraphNodeDataEx;
            //var curNodeSharp = NodeSharp as SGNode;
            SharpData.StartState = NodeDataGen.StartingState;
            SharpData.SetupStateMachine(reqList, transList);
            //initialize the Connections Dictionary
            SharpData.BuildConnectionDictionaryList();
            //check our ScriptableObject data and our Connection data to make sure they are the same Count
            if(ConnectedNodes.Count != NodeDataTemplate.Connections.Count)
            {
                Debug.LogWarning($"Connected Nodes Count does not match the NodeDataGen Connected Nodes Count");
                //fixing this
                ConnectedNodes.Clear();
                for(int i=0;i<NodeDataTemplate.Connections.Count;i++)
                {
                    ConnectedNodes.Add(allNodesInScene.Find(x=>x.NodeDataGen == NodeDataTemplate.Connections[i]));
                }
            }
            return SharpData;
        }
        /// <summary>
        // Setup the bare bones of the data for the nodeID by GameObject Instance ID
        /// </summary>
        /// <returns></returns>
        public SGNode InitialSetup()
        {
            if(SharpData==null)
            {
                SharpData = new SGNode(this.gameObject.GetInstanceID().ToString());
            }
            return SharpData;
        }
        
        /// <summary>
        /// Called from the Editor Script
        /// </summary>
        public void UpdateUnityFromTemplateData()
        {
            this.StartState = NodeDataGen.StartingState;
            //this.ConnectedNodes.Clear();
            //this.ConnectedNodes = editorConnectionsPassed;
        }
        /// <summary>
        /// This method replaces the EventsByType list with the runtime data that we probably read in from JSON
        /// </summary>
        public void ReplaceEventsByTypeWithRuntimeData()
        {
            /*
            for (int i = 0; i < RuntimeNode.StateTransitions.Count; i++)
            {
                var curStateTransition = RuntimeNode.StateTransitions[i];
                EventsByType.Clear();
                // now copy each RunTimeNode StateTransition back into the EventsByType list
                TransitionD tempData = new TransitionD();
                tempData.Transition = curStateTransition.Transition;
                tempData.OutcomeStatus = curStateTransition.OutcomeStatus;
                for (int j = 0; j < curStateTransition.UnityActionEvents.Count; j++)
                {
                    tempData.UnityActionEvents.Add(curStateTransition.UnityActionEvents[j]);
                }
                EventsByType.Add(tempData);
            }
            */
        }
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (NodeDataTemplate != null)
            {
                // Trigger editor update when NodeDataTemplate changes
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if(SharpData == null)
            {
                return;
            }
            Vector3 centerP = transform.position;
            // List<Vector3> endPoints = new List<Vector3>();

            Color curColor = FP_UtilityData.ReturnColorByStatus(SharpData.StartState);
            for (int i = 0; i < ConnectedNodes.Count; i++)
            {
                if (ConnectedNodes[i] != null)
                {
                    Vector3 nextS = ConnectedNodes[i].transform.position;
                    Vector3 startTan = new Vector3(centerP.x, centerP.y + 1 + (i * 2f), centerP.z);

                    Vector3 forwardV = (nextS - startTan).normalized;
                    UnityEditor.Handles.DrawBezier(centerP, nextS - (forwardV * 0.25f), startTan, nextS, curColor, null, 2f);

                    UnityEditor.Handles.color = curColor;
                    UnityEditor.Handles.ConeHandleCap(0, nextS - (forwardV * 0.25f), Quaternion.LookRotation(forwardV), 0.25f, EventType.Repaint);
                }
            }
            
            for (int j = 0; j < NodeDataGen.Requirements.Count; j++)
            {
                
                if (NodeDataGen.Requirements[j].RequirementTag != null)
                {
                    if (ConnectedNodes[j] != null)
                    {
                        Vector3 nextS = ConnectedNodes[j].transform.position;
                        //endPoints.Add(nextS);
                        Vector3 startTan = new Vector3(centerP.x, centerP.y + 1 + (j * 2f), centerP.z);

                        Vector3 forwardV = (nextS - startTan).normalized;
                        Color fromColor = FP_UtilityData.ReturnColorByStatus(SharpData.StartState);
                        UnityEditor.Handles.DrawBezier(centerP, nextS - (forwardV * 0.25f), startTan, nextS, fromColor, null, 2f);

                        UnityEditor.Handles.color = fromColor;
                        UnityEditor.Handles.DrawSolidDisc(nextS - (forwardV * 0.25f), forwardV, 0.25f);
                    }
                    
                }
            }
            
#endif
        }
        /// <summary>
        /// Help with debugging sequences
        /// </summary>

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (SharpData == null)
            {
                //Gizmos.DrawIcon()
                Gizmos.DrawIcon(transform.position, "/FP/Chain/error.jpg", true);
                return;
            }
            switch (SharpData.StartState)
            {
                case SequenceStatus.None:
                    Gizmos.DrawIcon(transform.position, "/FP/Chain/NA.png", true);
                    break;
                case SequenceStatus.Locked:
                    Gizmos.DrawIcon(transform.position, "/FP/Chain/locked.png", true);
                    break;
                case SequenceStatus.Unlocked:
                    Gizmos.DrawIcon(transform.position, "/FP/Chain/unlocked.png", true);
                    break;
                case SequenceStatus.Active:
                    Gizmos.DrawIcon(transform.position, "/FP/Chain/active.png", true);
                    break;
                case SequenceStatus.Finished:
                    Gizmos.DrawIcon(transform.position, "/FP/Chain/finished.png", true);
                    break;
            }

#endif
        }
    }
}

