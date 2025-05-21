using System.ComponentModel;

namespace sim756.RetryEx;

/// <summary>
/// Resilient retry mechanism.
/// </summary>
public class Retry
{
    /// <summary>
    /// Primary <see cref="Action"/>, to be retried.
    /// </summary>
    public Action? RetryAction { get; set; }

    /// <summary>
    /// Resilient exception handler <see cref="Action"/>.
    /// </summary>
    /// <seealso cref="Exception"/>
    public Action<Exception>? ExceptionAction { get; set; }

    /// <summary>
    /// Times the <see cref="RetryAction"/> will be re-invoked if <see cref="Exception"/> occurs.
    /// </summary>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryCount"/>
    public int Count { get; set; } = DefaultRetryOptions.DefaultRetryCount;

    /// <summary>
    /// Default delay between retries in milliseconds.
    /// </summary>
    /// <remarks>The default delay is 1000 ms, set by <see cref="DefaultRetryOptions.DefaultRetryDelay"/></remarks>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryDelay"/>
    public int Delay { get; set; } = DefaultRetryOptions.DefaultRetryDelay;

    /// <summary>
    /// When retrying, attempts are done either way with success or failure.
    /// </summary>
    /// <seealso cref="IsSuccessful"/>
    public bool IsCompleted { get; private set; } = false;

    /// <summary>
    /// Whether retrying <see cref="Count"/> (or set by parameter) times is done regardless with success or failure.
    /// </summary>
    public bool? IsCompletedWithSuccess { get; private set; } = null;

    /// <summary>
    /// Whether retrying <see cref="Count"/> (or set by parameter) times is done regardless with success or failure.
    /// </summary>
    public bool? IsSuccessful { get; private set; } = null;

    /// <summary>
    /// <para>
    /// Parameterless constructor for the Object-Initialization mechanism, use <see cref="Invoke()"/> method to execute the <see cref="RetryAction"/>.
    /// </para>
    /// <para>
    /// ⚠️ Requires manual invocation. <see cref="Invoke()"/>
    /// </para>
    /// </summary>
    /// <seealso cref="Invoke()"/>
    /// <seealso cref="RetryAction"/>
    /// <seealso cref="Count"/>
    /// <seealso cref="Delay"/>
    /// <seealso cref="ExceptionAction"/>
    public Retry()
    {
    }

    /// <summary>
    /// Constructor initializer, preferred for manual invocation.
    /// </summary>
    /// <param name="retryAction">Primary <see cref="Action"/> to be tried and retried.</param>
    /// <param name="count">How many times the primary <see cref="Action"/> will be retried.</param>
    /// <param name="delay">Delay between retries.</param>
    /// <param name="exceptionAction"><see cref="Action"/> to resiliently handles the exceptions (<see cref="Exception"/>) thrown in every try or retry. See method documentation, for example.</param>
    /// <param name="instantInvoke">Whether to <see cref="Invoke()"/> on construction. Default: false.</param>
    /// <param name="syncProperties">Whether to assign parameter values to the properties. Default: true.</param>
    /// <seealso cref="Invoke()"/>
    /// <seealso cref="RetryAction"/>
    /// <seealso cref="Count"/>
    /// <seealso cref="Delay"/>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryCount"/>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryDelay"/>
    /// <seealso cref="ExceptionAction"/>
    /// <remarks>
    ///     ⚠️ Requires manual invocation using <see cref="Invoke()"/> method if <see cref="instantInvoke"/> parameter is left to default false.
    /// </remarks>
    /// <code>
    /// // Example 1: Basic retry with exception handling
    /// Retry retry = new Retry(
    ///     retryAction: () => {
    ///         // Your action here
    ///         Console.WriteLine("Attempting operation...");
    ///     },
    ///     count: 3,
    ///     delay: 1000,
    ///     exceptionAction: (ex) => {
    ///         Console.WriteLine($"Failed attempt: {ex.Message}");
    ///     }
    /// );
    /// retry.Invoke();
    /// </code>
    /// OR,
    /// <code>
    /// // Example 2: Immediate execution with instantInvoke
    /// new Retry(
    ///     retryAction: () => Console.WriteLine("Immediate execution"),
    ///     count: 2,
    ///     instantInvoke: true
    /// );
    /// </code>
    [EditorBrowsable(EditorBrowsableState.Always)]
    public Retry(
        Action retryAction,
        int count = DefaultRetryOptions.DefaultRetryCount,
        int delay = DefaultRetryOptions.DefaultRetryDelay,
        Action<Exception>? exceptionAction = null,
        bool instantInvoke = false, 
        bool syncProperties = true)
    {
        if (syncProperties)
        {
            this.RetryAction = retryAction;
            this.ExceptionAction = exceptionAction;
            this.Count = count;
            this.Delay = delay;
        }

        if (instantInvoke)
        {
            ResilientRetry(retryAction ?? throw new NullReferenceException(), exceptionAction, count, delay);
        }
    }

    /// <summary>
    /// Internal resilient retry handler.
    /// </summary>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryCount"/>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryDelay"/>
    private void ResilientRetry(Action retryAction, Action<Exception>? exceptionAction = null,
        int count = DefaultRetryOptions.DefaultRetryCount, int delay = DefaultRetryOptions.DefaultRetryDelay)
    {
        for (int i = 0; i < count; i++)
        {
            try
            {
                try
                {
                    if (i > 0 && count > 1)
                    {
                        Thread.Sleep(delay);
                    }

                    retryAction.Invoke();

                    IsCompleted = true;
                    IsSuccessful = true;
                    IsCompletedWithSuccess = true;

                    return;
                }
                catch (Exception e)
                {
                    exceptionAction?.Invoke(e);
                }
            }
            catch
            {
                //suppress
            }
        }

        IsCompleted = true;
        IsSuccessful = false;
        IsCompletedWithSuccess = false;
    }

    /// <summary>
    /// Invokes the primary <see cref="Action"/>. 
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown when <see cref="RetryAction"/> is not initialized.</exception>
    /// <seealso cref="ExceptionAction"/>
    /// <seealso cref="Count"/>
    /// <seealso cref="Delay"/>
    /// <remarks>⚠️ Requires <see cref="RetryAction"/> to be initialized first.</remarks>
    public void Invoke()
    {
        ResilientRetry(RetryAction ?? throw new NullReferenceException(), ExceptionAction, Count, Delay);
    }

    /// <summary>
    /// Invokes the primary <see cref="Action"/> with specified retry count and delay.
    /// </summary>
    /// <param name="count">How many times the primary <see cref="Action"/> will be retried.</param>
    /// <param name="delay">Delay between retries in milliseconds. Default value is set by <see cref="DefaultRetryOptions.DefaultRetryDelay"/>.</param>
    /// <exception cref="NullReferenceException">Thrown when <see cref="RetryAction"/> is not initialized.</exception>
    /// <seealso cref="RetryAction"/>
    /// <seealso cref="ExceptionAction"/>
    /// <remarks>⚠️ Requires <see cref="RetryAction"/> to be initialized first.</remarks>
    public void Invoke(int count, int delay = DefaultRetryOptions.DefaultRetryDelay)
    {
        ResilientRetry(RetryAction ?? throw new NullReferenceException(), ExceptionAction, count, delay);
    }
}