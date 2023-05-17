package javareflectionapp;

/**
 *
 * @author std_string
 */
public class Request {
    public Request(String command, String arg) {
        this.command = command;
        this.arg = arg;
    }
    
    public String getCommand() {
        return command;
    }
    
    public String getArg() {
        return arg;
    }
    
    private final String command;
    private final String arg;
}
