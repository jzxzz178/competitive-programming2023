namespace MultiLockTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        var m = new MultiLock.MultiLock();
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}