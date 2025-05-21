namespace sim756.RetryEx;

/// <summary>
/// Resilient retry mechanism with return value and parameters.
/// </summary>
/// <typeparam name="TParameter">Parameter or collection of parameters as <see cref="Tuple"/>.
/// See <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples">C# Tuple documentation here</see>
/// </typeparam>
/// <typeparam name="TResult">Return type.</typeparam>
public class Retry<TParameter, TResult>
{
    /// <summary>
    /// Returns value assigned after invocation.
    /// </summary>
    public TResult? Result { get; private set; }

    /// <summary>
    /// Primary <see cref="Func{TParameter, TResult}"/>, to be retried.
    /// </summary>
    public Func<TParameter?, TResult>? RetryFunc { get; set; }

    /// <summary>
    /// Parameter or collection of parameters as <see cref="Tuple"/>.
    /// See <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples">C# Tuple documentation here</see>
    /// </summary>
    /// <remarks>Set null when no parameter is needed.</remarks>
    public TParameter? Parameters { get; set; }

    /// <summary>
    /// Resilient exception handler <see cref="Action"/>.
    /// </summary>
    /// <code>
    ///  (exception) =>
    ///  {
    ///      Console.WriteLine(exception.Message);
    ///  }
    /// </code>
    /// or, catch in detail by rethrowing the exception,
    /// <code>
    ///  (exception) =>
    ///  {
    ///      try
    ///      {
    ///          throw exception;
    ///      }
    ///      catch (IOException e)
    ///      {
    ///          //Add code here            
    ///      }
    ///      catch (NullReferenceException e)
    ///      {
    ///          //Add code here
    ///      }
    ///      //catch additional Exceptions here
    ///  }
    /// </code>
    /// <seealso cref="Exception"/>
    public Action<Exception>? ExceptionAction { get; set; }

    /// <summary>
    /// Times the <see cref="RetryFunc"/> will be re-invoked if <see cref="Exception"/> occurs.
    /// </summary>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryCount"/>
    public int Count { get; set; } = DefaultRetryOptions.DefaultRetryCount;

    /// <summary>
    /// Default delay between retries in milliseconds.
    /// </summary>
    /// <remarks>Default delay is 1000 ms, set by <see cref="DefaultRetryOptions.DefaultRetryDelay"/></remarks>
    /// <seealso cref="DefaultRetryOptions.DefaultRetryDelay"/>
    public int Delay { get; set; } = DefaultRetryOptions.DefaultRetryDelay;

    /// <summary>
    /// When retrying attempts are done either way with success or failure.
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
    /// Parameterless constructor for Object-Initialization and manual invocation, use <see cref="Invoke()"/> method to execute the <see cref="RetryFunc"/>.
    /// </para>
    /// <para>
    /// ⚠️ Requires manual invocation. <see cref="Invoke"/>
    /// </para>
    /// </summary>
    /// <seealso cref="Invoke()"/>
    /// <seealso cref="RetryFunc"/>
    /// <seealso cref="Count"/>
    /// <seealso cref="Delay"/>
    public Retry()
    {
    }

    /// <summary>
    /// Invokes <see cref="retryFunc"/> for <see cref="count"/> times with <see cref="delay"/> ms delay in
    /// between and return the result in <see cref="Result"/> property while resiliently handles <see cref="Exception"/>.
    /// </summary>
    /// <param name="retryFunc">Primary <see cref="Func{TParameter, TResult}"/></param>
    /// <param name="parameters">Parameters of <see cref="TParameter"/> type to be passed into the <see cref="Func{TParameter, TResult}"/></param>
    /// <param name="count">How many times the primary <see cref="Action"/> will be retried.</param>
    /// <param name="delay">Delay between retries.</param>
    /// <param name="exceptionAction"><see cref="Action"/> to resiliently handles the exceptions (<see cref="Exception"/>) thrown
    /// in every try or retry. See method documentation, for example.</param>
    /// <param name="instantInvoke">Whether to <see cref="Invoke()"/> on construction. Default: false.</param>
    /// <param name="syncProperties">Whether to assign parameter values to the properties. Default: true.</param>
    /// <code>
    /// int retVal = new Retry&lt;(int a, int b, int c)?, int&gt;
    ///     (
    ///         retryAction: (parameter) =>
    ///         {
    ///             //where exception may occur
    /// 
    ///             return (parameter?.a + parameter?.b + parameter?.c) ?? 0;
    ///         },
    ///         parameters: (a: 123, b: 123, c: 1234),
    ///         count: 3,
    ///         delay: 756,
    ///         exceptionAction: (exception) =>
    ///         {
    ///             //Caught exceptions
    ///         }
    ///     ).Result;
    /// </code>
    /// OR
    /// <code>
    /// int retVal = new Retry&lt;(int a, int b, int c)?, int&gt;
    ///     (
    ///         retryAction: (parameter) =>
    ///         {
    ///             //where exception may occur
    /// 
    ///             return (parameter?.a + parameter?.b + parameter?.c) ?? 0;
    ///         },
    ///         parameters: (a: 123, b: 123, c: 1234),
    ///         count: 3,
    ///         delay: 756,
    ///         exceptionAction: (exception) =>
    ///         {
    ///             try
    ///             {
    ///                 throw exception;
    ///             }
    ///             catch (IOException e)
    ///             {
    ///                 //Add code here            
    ///             }
    ///             catch (NullReferenceException e)
    ///             {
    ///                 //Add code here
    ///             }
    ///             //catch additional Exceptions here
    ///         }
    ///     ).Result;
    /// </code>
    public Retry(
        Func<TParameter?, TResult> retryFunc,
        TParameter? parameters,
        int count,
        int delay,
        Action<Exception>? exceptionAction,
        bool instantInvoke = false,
        bool syncProperties = true)
    {
        if (syncProperties)
        {
            this.RetryFunc = retryFunc;
            this.Parameters = parameters;
            this.ExceptionAction = exceptionAction;
            this.Count = count;
            this.Delay = delay;
        }

        if (instantInvoke)
        {
            ResilientRetry(retryFunc, parameters, exceptionAction, count, delay);
        }
    }

    /// <summary>
    /// Internal resilient retry handler.
    /// </summary>
    private void ResilientRetry(
        Func<TParameter?, TResult> retryFunc,
        TParameter? parameters,
        Action<Exception>? exceptionAction,
        int count = DefaultRetryOptions.DefaultRetryCount,
        int delay = DefaultRetryOptions.DefaultRetryDelay)
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

                    Result = retryFunc.Invoke(parameters);

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
    /// Invokes the primary <see cref="RetryFunc"/> with configured parameters.
    /// </summary>
    /// <returns>The result of type <typeparamref name="TResult"/> from the successful execution of <see cref="RetryFunc"/>.</returns> 
    /// <exception cref="NullReferenceException">Thrown when <see cref="RetryFunc"/> is not initialized.</exception>
    /// <seealso cref="RetryFunc"/>
    /// <seealso cref="ExceptionAction"/>
    /// <seealso cref="Count"/>
    /// <seealso cref="Delay"/>
    /// <remarks>⚠️ Requires <see cref="RetryFunc"/> to be initialized first.</remarks>
    public TResult? Invoke()
    {
        ResilientRetry(RetryFunc ?? throw new NullReferenceException(), Parameters, ExceptionAction, Count, Delay);
        return Result;
    }

    /// <summary>
    /// Invokes the primary <see cref="RetryFunc"/> with specified retry count and delay.
    /// </summary>
    /// <param name="count">How many times the primary <see cref="RetryFunc"/> will be retried.</param>
    /// <param name="delay">Delay between retries in milliseconds. Default value is set by <see cref="DefaultRetryOptions.DefaultRetryDelay"/>.</param>
    /// <returns>The result of type <typeparamref name="TResult"/> from the successful execution of <see cref="RetryFunc"/>.</returns>
    /// <exception cref="NullReferenceException">Thrown when <see cref="RetryFunc"/> is not initialized.</exception>
    /// <seealso cref="RetryFunc"/>
    /// <seealso cref="ExceptionAction"/>
    /// <remarks>⚠️ Requires <see cref="RetryFunc"/> to be initialized first.</remarks>
    public TResult? Invoke(int count, int delay = DefaultRetryOptions.DefaultRetryDelay)
    {
        ResilientRetry(RetryFunc ?? throw new NullReferenceException(), Parameters, ExceptionAction, count, delay);
        return Result;
    }
}