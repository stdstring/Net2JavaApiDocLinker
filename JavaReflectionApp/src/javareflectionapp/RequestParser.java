package javareflectionapp;

/**
 *
 * @author std_string
 */
public class RequestParser {
    public Request parse(String input) {
        int argDelimiter = input.indexOf(RequestCommandDef.commandArgDelimiter);
        String command = argDelimiter == -1 ? input : input.substring(0, argDelimiter);
        String arg  = argDelimiter == -1 ? null : input.substring(argDelimiter + 1);
        return new Request(command, arg);
    }
}
