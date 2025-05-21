# Resilient Retry Mechanism

###### Example 1:

```c#
new Retry(
    retryAction: () =>
    {
        // write your code here, thrown exception will be retried
    },
    count: 10, // number of retries
    delay: 1000, // delays between retries in milliseconds
    exceptionAction: (exception) =>
    {
        // handle exception here
        // this action will be called for each retry
        // you can log the exception or perform any other action
    }
);
```

OR, shorter form,

###### Example 2:

```c#
new Retry(() =>
    {
        // write your code here, thrown exception will be retried
    },
    10, // number of retries
    1000, // delays between retries in milliseconds
    (ex) =>
    {
        // handle exception here
    }
);
```

Catching specific exceptions

###### Example 3:

```c#
new Retry(
    retryAction: () =>
    {
        //where exception could be thrown
    },
    count: 10,
    delay: 5000,
    exceptionAction: (exception) =>
    {
        try
        {
            throw exception; // rethrow exception to recatch specific exceptions.
        }
        catch (IOException e)
        {
            //Add code here            
        }
        catch (NullReferenceException e)
        {
            //Add code here
        }
        //catch additional Exceptions here
    }
);
```

With parameters and return,

###### Example 4:

```c#
int retVal = new Retry<(int a, int b, int c), int>
        (
            retryFunc: (parameters) =>
            {
                // where exception could be thrown
                return (parameters.a + parameters.b + parameters.c);
            },
            parameters: (a: 123, b: 123, c: 1234),
            count: 3,
            delay: 756,
            exceptionAction: (exception) =>
            {
                try
                {
                    throw exception; // rethrow exception to recatch specific exceptions.
                }
                catch (IOException e)
                {
                    //Add code here            
                }
                catch (NullReferenceException e)
                {
                    //Add code here
                }
                //catch additional Exceptions here
            }
        ).Result;
```
