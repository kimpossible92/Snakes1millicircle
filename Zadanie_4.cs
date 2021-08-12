using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public struct ApiSetup<T>
    {
		internal void SetupObjectA()
        {

		}

		internal void SetupObjectB()
        {
			
        }
    }
	public class TemplateTest<T> where T : IEnumerable { }

   class Api
	{
		object[] arr = new object[10]; int index = 0;
		public ApiSetup<T> For<T>(T obj)
		{
			arr[index++] = obj;
			return new ApiSetup<T>();
		}
		public object get(int index)
		{
			if (index < this.index && index >= 0)
			{
				return arr[index];
			}
			else
			{
				return default(object);
			}
		}
		public int Count()
		{
			return index;
		}
	}
	interface ISomeInterfaceA
	{
		
	}
	interface ISomeInterfaceB
	{
		
	}
	public class ObjectA : ISomeInterfaceA
	{
	}
	public class ObjectB : ISomeInterfaceB
	{
	}
	class SomeClass
	{
		public void Setup()
		{
			Api apiObject = new Api();
			apiObject.For(new ObjectA()).SetupObjectA();
			apiObject.For(new ObjectB()).SetupObjectB();
		}
	}
}