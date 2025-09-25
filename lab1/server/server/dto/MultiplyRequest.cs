namespace server.dto
{
    public class MultiplyRequest
    {
        public float[][] Chunk { get; set; }
        public float[][] OrigMatrix { get; set; }
        public int RowStart { get; set; }
    }
}
