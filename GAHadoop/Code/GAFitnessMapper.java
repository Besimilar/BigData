package midTerm.HHW;

 import java.io.IOException; 
  
 import org.apache.hadoop.io.Writable; 
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.Mapper; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reporter;
 import org.apache.hadoop.io.Text; 
  
  
 public class GAFitnessMapper extends MapReduceBase implements Mapper<WritableComparable, Writable, Text, Text> { 
	 
	 public void map(WritableComparable key, Writable value, 
                   OutputCollector<Text, Text> output, Reporter reporter) throws IOException { 
  
	 String chromo = ((Text)value).toString(); 
	 
	 // if it contains generation Index
	 String[] rowChromo = chromo.split("\t");
	 if (rowChromo.length == 2) 
		 {chromo = rowChromo[1]; Chromosome.gen = Integer.parseInt(rowChromo[0]);} 
	 else Chromosome.gen = 0;
	 
     Chromosome cs = new Chromosome(chromo);
     cs.scoreChromo(Chromosome.target);
     
     // see whether a solution exists in current generation
     if (cs.total == Chromosome.target && cs.isValid()) 
     { 
    	 	String solution = "Solution: " + cs.decodeChromo() + " in Generations: " + Chromosome.gen;
    	 	output.collect(new Text(cs.chromo.toString() + " - "), new Text(solution));
    	 	Chromosome.findSolution = true;
     }
   
     // collect each chromosome's score (fitness)
     Text toEmit = new Text((new Double(cs.score)).toString()); 
   
     output.collect(new Text(chromo), toEmit); 

   } 
	 
	 
 } 
 