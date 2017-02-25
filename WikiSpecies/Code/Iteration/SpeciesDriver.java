package assign5;

import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.FloatWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapred.FileInputFormat;
import org.apache.hadoop.mapred.FileOutputFormat;
import org.apache.hadoop.mapred.JobClient;
import org.apache.hadoop.mapred.JobConf;

public class SpeciesDriver {
	
	public static void main(String[] args)  throws Exception
	{ 
		
		String input = args[0];
		String output = args[1];
		
		 // ------- GraphBuilder job
		JobClient client = new JobClient();
		/*JobConf conf1 = new JobConf(SpeciesDriver.class);
		conf1.setJobName("WikiSpecies P/R Graph Builder");

		conf1.set("xmlinput.start", "<page>");
		conf1.set("xmlinput.end", "</page>");

		// TODO what is the purpose of io.serializations?
		conf1.set(
				"io.serializations",
				"org.apache.hadoop.io.serializer.JavaSerialization,org.apache.hadoop.io.serializer.WritableSerialization");

		// specify a map function
		conf1.setMapperClass(SpeciesGraphBuilderMapper.class);
		// specify a reducer function
		conf1.setReducerClass(SpeciesGraphBuilderReducer.class);
		// optionally add a combiner
		// conf.setCombinerClass(XReduce.class);

		// for test
		conf1.setNumReduceTasks(1);

		conf1.setInputFormat(XmlInputFormat.class);
		// call a custom formatter to produce XML wrapped output
		// jobConf.setOutputFormat(XmlOutputFormat.class);
		// specify output types
		conf1.setOutputKeyClass(Text.class);
		conf1.setOutputValueClass(Text.class);

		Path outPath = new Path(output);
		FileInputFormat.addInputPath(conf1, new Path(input));
		FileOutputFormat.setOutputPath(conf1, outPath);

		FileSystem dfs = FileSystem.get(outPath.toUri(), conf1);
		if (dfs.exists(outPath)) {
			dfs.delete(outPath, true);
		}

		client.setConf(conf1);
		try {
			JobClient.runJob(conf1);
		} catch (IOException ex) {
			Logger.getLogger(SpeciesDriver.class.getName()).log(
					Level.SEVERE, null, ex);
		}
		*/
	
	     //--------------- Iteration
		 // input = output;
	     String filePrfx = output;
	   
	     int round = 1; // set iteration number = 10
	     
	     try {
	    	 	round = Integer.parseInt(args[2]);
	     }
	     catch (Exception ex) {
	    	 	round = 10;
	     }
	    
	     
	     for (int i = 0; i < round; i++) {
	    	 	
	    	 	 JobConf conf2 = new JobConf(SpeciesDriver.class);
		     conf2.setJobName("Species Iter" + (i+1)); 
		  
		     conf2.setNumReduceTasks(1); // this code is no use, except 0???
		  
			 conf2.setOutputKeyClass(Text.class); 
			 conf2.setOutputValueClass(Text.class); 
		  
			 output = (filePrfx + (i+1));
			 FileInputFormat.setInputPaths(conf2, new Path(input));
			 FileOutputFormat.setOutputPath(conf2, new Path(output));
		  
			 conf2.setMapperClass(SpeciesIterMapper.class); 
			 conf2.setReducerClass(SpeciesIterReducer.class); 
			 conf2.setCombinerClass(SpeciesIterReducer.class); 
			  
			 client.setConf(conf2); 
			 try { 
			 JobClient.runJob(conf2); 
			 } catch (Exception e) { 
			 e.printStackTrace(); 
			 } 
			 
			 input = output;
	     }
	     

		//----------- View
		JobConf conf3 = new JobConf(SpeciesDriver.class);
		conf3.setJobName("Species Viewer"); 

		conf3.setOutputKeyClass(FloatWritable.class); 
		conf3.setOutputValueClass(Text.class); 

		FileInputFormat.setInputPaths(conf3, new Path(input));
		FileOutputFormat.setOutputPath(conf3, new Path(filePrfx + "-final"));
		
		conf3.setMapperClass(SpeciesViewerMapper.class); 
		conf3.setReducerClass(org.apache.hadoop.mapred.lib.IdentityReducer.class); 
		
		client.setConf(conf3); 
		try { 
		  JobClient.runJob(conf3); 
		} catch (Exception e) { 
		  e.printStackTrace(); 
		} 
		
	}
	
}
