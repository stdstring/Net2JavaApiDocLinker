package javareflectionapp;

import java.io.IOException;

/**
 *
 * @author std_string
 */
public interface RequestProcessor {
    String process(Request request) throws IOException, ClassNotFoundException;
}
