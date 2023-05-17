package javareflectionapp;

import java.util.Arrays;
import org.junit.Assert;
import org.junit.Test;

/**
 *
 * @author std_string
 */
public class ResponseBuilderTest {
    @Test public void build() {
        String multiPartResponse = responseBuilder.build(Arrays.asList("part1", "part2", "part3"));
        Assert.assertNotNull(multiPartResponse);
        Assert.assertEquals("part1;part2;part3", multiPartResponse);
        String singlePartResponse = responseBuilder.build(Arrays.asList("part"));
        Assert.assertNotNull(singlePartResponse);
        Assert.assertEquals("part", singlePartResponse);
        String emptyResponse = responseBuilder.build(Arrays.<String>asList());
        Assert.assertNotNull(emptyResponse);
        Assert.assertEquals("", emptyResponse);
    }
    
    private final ResponseBuilder responseBuilder = new ResponseBuilder();
}
