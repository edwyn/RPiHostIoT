using System;
using System.ServiceModel;

[ServiceContract]
public interface IMyFirstService
{
    [OperationContract]
    int GetNumberWords(string s);
}