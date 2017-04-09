package midTerm.HHW;

 import java.io.IOException;
import org.apache.hadoop.io.Writable; 
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.Mapper; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reporter;
 import org.apache.hadoop.io.Text; 
  
  
 public class GAReproduceMapper extends MapReduceBase implements Mapper<WritableComparable, Writable, Text, Text> { 
	 
	 {	
		 Chromosome.gen++; // reproduce a new generation
	 }
	 
	 
	 public void map(WritableComparable key, Writable value, 
                   OutputCollector<Text, Text> output, Reporter reporter) throws IOException { 
  
     // read data:
	 // format: groupIndex - chromoIndex - BinaryExpression
	 // - chromoIndex2 - BinaryExpression2
	 String chromos = ((Text)value).toString(); 
	 String[] chromosDetail = chromos.split("\t");
	 
	 Chromosome cs1 = new Chromosome(chromosDetail[2]);
	 Chromosome cs2 = new Chromosome(chromosDetail[4]);
	 
	 cs1.crossOver(cs2);
	 cs1.mutate();
	 cs2.mutate();
	 
	 Text index = new Text(chromosDetail[0]);
	
	 // Map Format:
	 // GroupIndex - BinaryExpression
	 output.collect(index, new Text(cs1.chromo.toString()));
	 output.collect(index, new Text(cs2.chromo.toString()));


   } 
	 
	 
 } 
 