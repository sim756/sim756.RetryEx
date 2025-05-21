namespace sim756.RetryEx;

public static class DefaultRetryOptions
{
    /// <summary>
    /// Default retry count.
    /// </summary>
    /// <value>3 times</value>
    public const int DefaultRetryCount = 3;

    /// <summary>
    /// Default delay between retries in milliseconds.
    /// </summary>
    /// <value>1000 ms</value>
    public const int DefaultRetryDelay = 1000;
}