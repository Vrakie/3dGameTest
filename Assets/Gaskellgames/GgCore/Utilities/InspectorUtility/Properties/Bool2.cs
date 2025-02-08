using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [System.Serializable]
    public class Bool2
    {
        [SerializeField]
        public bool x;
        
        [SerializeField]
        public bool y;

        public Bool2()
        {
            this.x = false;
            this.y = false;
        }

        public Bool2(bool x, bool y)
        {
            this.x = x;
            this.y = y;
        }

        public bool LogicOutput(GgMaths.LogicGate logicType)
        {
            return GgMaths.LogicGateOutputValue(this.x, this.y, logicType);
        }

        public void None()
        {
            this.x = false;
            this.y = false;
        }

        public void FreezeAll()
        {
            this.x = true;
            this.y = true;
        }

    } // class end
}
