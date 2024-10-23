namespace Assets.Scripts.RootS
{
    public class Edge
    {
        public RootNode nodeA;
        public RootNode nodeB;

        public Edge(RootNode nodeA, RootNode nodeB)
        {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
        }
    }
}
