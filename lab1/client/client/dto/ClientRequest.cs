namespace client.dto
{
    public class ClientRequest 
    { 
        public float[][] Chunk { get; set; } 
        public float[][] OrigMatrix { get; set; } 
        public int RowStart { get; set; } 
    }
}
