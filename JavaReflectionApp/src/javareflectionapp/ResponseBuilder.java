package javareflectionapp;

import java.util.List;

/**
 *
 * @author std_string
 */
public class ResponseBuilder {
    public String build(List<String> responseParts) {
        StringBuilder sbuilder = new StringBuilder();
        for(String part : responseParts) {
            if(sbuilder.length() > 0) sbuilder.append(responsePartDelimiter);
            sbuilder.append(part);
        }
        return sbuilder.toString();
    }
    
    private static final String responsePartDelimiter = ";";
}
