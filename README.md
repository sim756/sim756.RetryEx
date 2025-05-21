# Resilient Retry Mechanism

## Examples

### Example 1
Retry with Exception handling and executing only on Invoke() call.

```c#
Retry numberRetry = new Retry(
    retryAction: () =>
    {
        WriteExampleHeader();

        int randomNumber = GenerateRandomNumber();
        Console.WriteLine($"Random number: {randomNumber}");
    },
    exceptionAction: (exception) =>
    {
        Console.WriteLine($"Exception: {exception.Message}");
    }
);

numberRetry.Invoke();
```

### Example 2
Retry with Exception handling with specified delays and retry count; no manual invocation needed, it auto executes.

```c#
new Retry
(
    retryAction: () =>
    {
        WriteExampleHeader();
        
        int randomNumber = GenerateRandomNumber();
        Console.WriteLine($"Random number: {randomNumber}");
    },
    count: 10, // retying 10 times until success or all 10 retries fails.
    delay: 1000, // 1-second delay between retries.
    exceptionAction: (exception) =>
    {
        Console.WriteLine($"Exception: {exception.Message}");
    }
).Invoke();
```

### Example 3
Retry with parameters and return value.

```c#
int retValExample3 = new Retry<(int para1, int para2, int para3), int>
(
    param =>
    {
        WriteExampleHeader();

        int randomNumber = GenerateRandomNumber();
        int result = randomNumber + param.para1 + param.para2 + param.para3;
        return result; // Returning result to the retVal.
    },
    (10, 100, 1000), // parameter values
    2, // retry count
    1000, // delay in milliseconds
    ex =>
    {
        Console.WriteLine($"Exception: {ex.Message}");
    }
).Invoke();

Console.WriteLine($"retValExample3: {retValExample3}");
```

### Example 4
Retry with parameters and return value.

```c#
Retry<(int para1, int para2, int para3), int> retryExample4
    = new
    (
        param =>
        {
            WriteExampleHeader();
            
            int randomNumber = GenerateRandomNumber();
            int result = randomNumber + param.para1 + param.para2 + param.para3;
            return result; // Returning result to the retVal.
        },
        (10, 100, 1000), // parameter values
        count: 2, // retry count
        delay: 1000, // delay in milliseconds
        ex =>
        {
            Console.WriteLine($"Exception: {ex.Message}"); 
        }
    );

retryExample4.Invoke();

Console.WriteLine($"retryExample4.Result: {retryExample4.Result}");
```

### Example 5
Retry with parameters and return value, and with additional custom exception handling.

```c#
Retry<(int para1, int para2, int para3), int> retryExample5
    = new(
        param =>
        {
            WriteExampleHeader();

            int randomNumber = GenerateRandomNumber();
            int result = randomNumber + param.para1 + param.para2 + param.para3;
            return result; // Returning result to the retVal.
        },
        (10, 100, 1000), // parameter values
        2, // retry count
        1000, // delay in milliseconds
        exception =>
        {
            try
            {
                throw exception;
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    );

retryExample5.Invoke();
Console.WriteLine($"retryExample5.Result: {retryExample5.Result}");
```