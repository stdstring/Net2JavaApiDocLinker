package javareflectionapp;

import java.util.HashMap;
import java.util.Map;

/**
 *
 * @author std_string
 */
public class RequestProcessorFactory {
    public RequestProcessor get(Request request) {
        return factoryMap.get(request.getCommand());
    }
    
    private final Map<String, RequestProcessor> factoryMap = new HashMap<String, RequestProcessor>();
    {
        factoryMap.put(RequestCommandDef.getClassesFromJar, new ClassListRequestProcessor());
        factoryMap.put(RequestCommandDef.getMembersForClass, new ClassMemberRequestProcessor());
    }
}
