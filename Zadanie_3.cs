interface ISomeInterface
{
 void Call();
}
struct SomeStruct : ISomeInterface
{
 public void Call()
 { }
}
class SomeClass
{
 public void Run()
 {
 var someStruct = new SomeStruct();
 SomeMethod(someStruct);
 }
 public void SomeMethod(ISomeInterface @interface)
 {
 @interface.Call();
 }
}

struct SomeStruct : IDisposable
{
	void IDisposable.Dispose() {}
}
class SomeClass
{
 public void Run()
 {
 var some = new SomeStruct();
 //...
 ((IDisposable)SomeStruct).Dispose();
 }
 public void Obects()
 {
	var list = new[] { 1, 2 }.ToList();
	list.Add("ой");

	var list = new[] { (object)1, 2 }.ToList();
	list.Add("ой");
	Animal animal = nya ? new Cat() : new Dog(); // не компилируется в C# <9
	Animal animal = nya ? (Animal)new Cat() : new Dog(); // компилируется

	Animal animal = nya ? new Cat() : new Dog(); // компилируется в C#9
	var animal = nya ? new Cat() : new Dog(); // не компилируется в C#9
	var animal = nya ? (Animal)new Cat() : new Dog(); // компилируется
 }
  static public IEnumerable<R> MultiZip<T, R>(
     this IEnumerable<List<T>> sequences,
     Func<IEnumerable<T>, R> resultSelector)
 {
     var enumerators =
             sequences.Select(s => (IEnumerator<T>)s.GetEnumerator()).ToList();
     try
     {
         while (enumerators.All(en => en.MoveNext()))
             yield return resultSelector(enumerators.Select(en => en.Current));
     }
     finally
     {
         foreach (var en in enumerators)
             en.Dispose();
     }
 }
}