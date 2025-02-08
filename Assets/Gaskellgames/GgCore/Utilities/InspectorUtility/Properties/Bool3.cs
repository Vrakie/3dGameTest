using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [System.Serializable]
    public class Bool3
    {
        [SerializeField]
        public bool x;
        
        [SerializeField]
        public bool y;
        
        [SerializeField]
        public bool z;

        public Bool3()
        {
            this.x = false;
            this.y = false;
            this.z = false;
        }

        public Bool3(bool x, bool y, bool z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool LogicOutput(GgMaths.LogicGate logicType)
        {
            bool xyOutput = GgMaths.LogicGateOutputValue(this.x, this.y, logicType);
            return GgMaths.LogicGateOutputValue(xyOutput, this.z, logicType);
        }

        public void None()
        {
            this.x = false;
            this.y = false;
            this.z = false;
        }

        public void FreezeAll()
        {
            this.x = true;
            this.y = true;
            this.z = true;
        }

    } // class end
}
