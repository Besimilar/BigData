package midTerm.HHW;

 import java.io.IOException; 
 import java.util.Iterator; 
  
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reducer; 
 import org.apache.hadoop.mapred.Reporter; 
 import org.apache.hadoop.io.Text; 
  
 public class GAReproduceReducer extends MapReduceBase implements Reducer<WritableComparable, Text, Text, Text> { 
  
	 
	 public void reduce(WritableComparable key, Iterator values, 
                      OutputCollector output, Reporter reporter) throws IOException { 
		 
		 String toEmit = "";
		 
		 while (values.hasNext()) { 
			 
		   // Map Format:
		   // GroupIndex - BinaryExpression
	    	 
	       String currChromo = ((Text)values.next()).toString();
	       String gen = new Integer(Chromosome.gen).toString();
	      
	       output.collect(gen, new Text(currChromo)); 
	 
	     }
		 
		 
	     
   } 
 } 
 