package javareflectionapp;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

/**
 *
 * @author std_string
 */
public class JavaReflectionApp {

    public static void main(String[] args) throws IOException, ClassNotFoundException {
        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        RequestParser parser = new RequestParser();
        RequestProcessorFactory processorFactory = new RequestProcessorFactory();
        while(true) {
            String requestString = reader.readLine();
            if(requestString.equals(exitCommand))
                break;
            Request request = parser.parse(requestString);
            RequestProcessor processor = processorFactory.get(request);
            String response = request == null ? "" : processor.process(request);
            System.out.println(response);
        }
        reader.close();
    }
    
   private static final String exitCommand = "exit";
}
