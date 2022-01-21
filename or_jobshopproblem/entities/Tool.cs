namespace or_jobshopproblem.entities
{
    internal class Tool
    {
        public int ToolId { get; set; }
        
        public int SawLife { get; set; }

        public int SawLifeAfterRenew { get; set; }

        public int SawLifeRenewDownTime { get; set; }

        //public IntVarWrapper? SawLife { get; set; }


        public Tool(int id, int blwc, int blrw)
        {
            this.ToolId = id;
        }

        public void setSawLife(string name, int lb, int up)
        {
            //this.SawLife = new IntVarWrapper(name, lb, up);
        }
    }
}
