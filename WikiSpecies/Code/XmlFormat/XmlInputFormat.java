package assign5;

import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.apache.hadoop.fs.FSDataInputStream;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.DataOutputBuffer;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapred.FileInputFormat;
import org.apache.hadoop.mapred.FileSplit;
import org.apache.hadoop.mapred.InputSplit;
import org.apache.hadoop.mapred.JobConf;
import org.apache.hadoop.mapred.RecordReader;
import org.apache.hadoop.mapred.Reporter;

//import org.apache.commons.logging.Log; // import commons-logging-1.2.jar

/**
 * Reads records that are delimited by a specific begin/end tag.
 */
public class XmlInputFormat extends FileInputFormat<LongWritable, Text> {
	public static final String START_TAG_KEY = "xmlinput.start";
	public static final String END_TAG_KEY = "xmlinput.end";

	@Override
	public RecordReader<LongWritable, Text> getRecordReader(
			InputSplit inputSplit, JobConf jobConf, Reporter reporter)
			throws IOException {

		reporter.setStatus(inputSplit.toString());
		return new XmlRecordReader(jobConf, (FileSplit) inputSplit);
	}

	public static class XmlRecordReader implements
			RecordReader<LongWritable, Text> { // inner class
		private byte[] startTag;
		private byte[] endTag;
		private long start;
		private long end;
		private FSDataInputStream fsin;
		private DataOutputBuffer buffer = new DataOutputBuffer();
		
		public XmlRecordReader(JobConf jobConf, FileSplit inputSplit)
				throws IOException {
			FileSplit fileSplit = (FileSplit) inputSplit;
			startTag = jobConf.get(START_TAG_KEY).getBytes("utf-8");
			endTag = jobConf.get(END_TAG_KEY).getBytes("utf-8");

			start = fileSplit.getStart(); // start position
			end = start + fileSplit.getLength(); // end position
			Path path = fileSplit.getPath();
			
			try {
				FileSystem fs = path.getFileSystem(jobConf);
				fsin = fs.open(path);
				fsin.seek(start); // seek from start position

			} catch (IOException ex) {
				System.out.println("IO exception in Input Formatter");
				Logger.getLogger(XmlInputFormat.class.getName()).log(Level.SEVERE,
						null, ex);
			}
		}

		@Override
		public LongWritable createKey() {
			return new LongWritable();
		}

		@Override
		public Text createValue() {
			return new Text();
		}

		@Override
		public long getPos() throws IOException {
			return fsin.getPos();
		}

		@Override // extract content
		public boolean next(LongWritable key, Text value) throws IOException {
			if (fsin.getPos() < end) { // if not reach end position
				if (readUntilMatch(startTag, false)) { // if read startTag
					try {
						buffer.write(startTag); // write startTag in buffer
						if (readUntilMatch(endTag, true)) { // if read endTag
							// set value to content between startTag and endTag
							value.set(buffer.getData(), 0, buffer.getLength()); 
							key.set(fsin.getPos()); // set key to position
							return true;
						}
					} finally {
						buffer.reset(); // reset buffer to buffer next context
					}
				}
			}
			return false;
		}

		@Override
		public float getProgress() throws IOException {
			return (fsin.getPos() - start) / (float) (end - start);
		}

		@Override
		public void close() throws IOException {
			fsin.close();
		}

		private boolean readUntilMatch(byte[] match, boolean withinBlock)
				throws IOException {
			int i = 0;
			while (true) {
				int b = fsin.read();
				// end of file:
				if (b == -1)
					return false;
				// save to buffer:
				if (withinBlock)
					buffer.write(b);

				// check if we're matching:
				if (b == match[i]) {
					i++;
					if (i >= match.length)
						return true;
				} else
					i = 0;

				// see if we've passed the stop point:
				if (!withinBlock && i == 0 && fsin.getPos() >= end)
					return false;
			}
		}
	}

}