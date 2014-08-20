#Overview
Contains various utility classes useful in the financial services industry in nordic countries.

##Class: [BankAccountSe](Avida.FinancialUtility/Bank/Se/BankAccountSe.cs)
Can validate and parse bank account numbers for all swedish banks according to the specification from BGC (http://www.bgc.se/upload/Gemensamt/Trycksaker/Manualer/BG910.pdf).

Note:
There are some older swedbank account numbers that are real but have an invalid check digit. These will be rejected by this class.

###Parsing an account number:
```c#
var a = BankAccountSe.CreateBankAccount("8888,1234 567-4");

//Result
Assert.AreEqual("Swedbank", a.Bank);
Assert.AreEqual("8888", a.ClearingNumber);
Assert.AreEqual("12345674", a.AccountNumber);
```

###Validating an account number:
```c#
var isValid = BankAccountSe.IsValidBankAccount("8888,1234 567-4");

//Result
Assert.IsTrue(isValid);
```
