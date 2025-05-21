namespace sim756.RetryEx.RetryExSample;

internal class Program
{
    private static void Main(string[] args)
    {
        // Example 1: Retry with Exception handling and executing only on Invoke() call.
        {
            Console.WriteLine("Example 1");

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
        }

        WriteConsoleGap();

        // Example 2: Retry with Exception handling with specified delays and retry count; no manual invocation needed, it auto executes.
        {
            Console.WriteLine("Example 2");

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
        }

        WriteConsoleGap();

        // Example 3: Retry with parameters and return value.
        {
            Console.WriteLine("Example 3");

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
        }

        WriteConsoleGap();

        // Example 4: Retry with parameters and return value.
        {
            Console.WriteLine("Example 4");
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
        }

        WriteConsoleGap();

        // Example 5: Retry with parameters and return value.
        Console.WriteLine("Example 5");

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
        
        
        Console.Write("\n\nPress any key to continue . . . ");
        Console.ReadKey();
    }

    private static void WriteConsoleGap()
    {
        Console.WriteLine("\n");
    }
    
    private static void WriteExampleHeader()
    {
        Console.WriteLine(
            "Generating random number, dividing the number by 3, throwing exception if there's any reminder.");
    }

    /// <summary>
    /// This private static method generates a random number between 1 and 99, prints it to the console, and then
    /// performs a divisibility check by 3 - if the number isn't divisible by 3, it throws an
    /// <see cref="InvalidDataException"/>, otherwise it prints "0 reminder" and returns the generated number;
    /// this behavior makes it suitable for demonstrating retry mechanisms since it will only successfully return
    /// numbers that are divisible by 3.
    /// </summary>
    /// <returns>The method returns a random number between 1 and 99 that is divisible by 3, otherwise throws
    /// an <see cref="InvalidDataException"/>.
    /// </returns>
    /// <exception cref="InvalidDataException"></exception>
    private static int GenerateRandomNumber()
    {
        int randomNumber = new Random().Next(1, 100);
        Console.WriteLine($"Random number: {randomNumber}");
        if (randomNumber % 3 != 0)
        {
            throw new InvalidDataException("Reminder available! Throwing Exception.");
        }
        else
        {
            Console.WriteLine("0 reminder.");
        }

        return randomNumber;
    }
}