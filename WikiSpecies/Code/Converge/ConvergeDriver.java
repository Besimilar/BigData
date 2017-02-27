package assign5;

 import org.apache.hadoop.fs.Path; 
 import org.apache.hadoop.io.IntWritable; 
 import org.apache.hadoop.io.Text; 
 import org.apache.hadoop.mapred.JobClient; 
 import org.apache.hadoop.mapred.JobConf; 
 import org.apache.hadoop.mapred.Mapper; 
 import org.apache.hadoop.mapred.Reducer; 

import org.apache.hadoop.mapred.FileInputFormat;
import org.apache.hadoop.mapred.FileOutputFormat;
  
  
 public class ConvergeDriver { 
  
 public static void main(String[] args) { 
	 
	 JobClient client = new JobClient(); 
	 JobConf conf = new JobConf(ConvergeDriver.class); 
	 conf.setJobName("Species Iter Converge");  
	 conf.setNumReduceTasks(1); 
	 conf.setOutputKeyClass(Text.class); 
	 conf.setOutputValueClass(Text.class); 
	  
	 if (args.length < 1) { 
	 System.out.println("Usage: <input path> <output path>"); 
	 System.exit(0); 
	 } 
	
	 FileInputFormat.setInputPaths(conf, new Path(args[0]));
	 FileOutputFormat.setOutputPath(conf, new Path(args[1]));
	  
	 conf.setMapperClass(SpeciesConvergeMapper.class); 
	 conf.setReducerClass(SpeciesConvergeReducer.class); 
	 conf.setCombinerClass(SpeciesConvergeReducer.class); 
	  
	 client.setConf(conf); 
	 try { 
	 JobClient.runJob(conf); 
	 } catch (Exception e) { 
	 e.printStackTrace(); 
	 } 
 
	 // for Sum Converge Job
 	JobConf conf1 = new JobConf(ConvergeDriver.class); 
 	 conf1.setJobName("Species Iter Converge Sum");  
	 conf1.setNumReduceTasks(1); 
	 conf1.setOutputKeyClass(Text.class); 
	 conf1.setOutputValueClass(Text.class); 
	 
	 FileInputFormat.setInputPaths(conf1, new Path(args[1]));
	 FileOutputFormat.setOutputPath(conf1, new Path(args[1] + "-Sum"));
	  
	 conf1.setMapperClass(SpeciesConvergeMapper2.class); 
	 conf1.setReducerClass(SpeciesConvergeReducer2.class); 
	 conf1.setCombinerClass(SpeciesConvergeReducer2.class); 
	  
	 client.setConf(conf1); 
	 try { 
	 JobClient.runJob(conf1); 
	 } catch (Exception e) { 
	 e.printStackTrace(); 
	 } 
	 
	 
 
 
 } 
 } 
 