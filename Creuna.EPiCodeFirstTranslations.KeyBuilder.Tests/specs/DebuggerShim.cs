using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;
using NUnit.Framework;

/*
 * Howdy,
 * 
 * This is NSpec's DebuggerShim.  It will allow you to use TestDriven.Net or Resharper's test runner to run
 * NSpec tests that are in the same Assembly as this class.  
 * 
 * It's DEFINITELY worth trying specwatchr (http://nspec.org/continuoustesting). Specwatchr automatically
 * runs tests for you.
 * 
 * If you ever want to debug a test when using Specwatchr, simply put the following line in your test:
 * 
 *     System.Diagnostics.Debugger.Launch()
 *     
 * Visual Studio will detect this and will give you a window which you can use to attach a debugger.
 */

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{

//[TestFixture]
    public class DebuggerShim
    {


        [Test]
        public void specifications()
        {
            var tagOrClassName = "KeyBuilder";

            // GetType().Assembly.GetTypes().ToList().ForEach(x => System.Console.Out.WriteLine($"type: {x}"));
            var types = GetType().Assembly.GetTypes();
            // OR
            // var types = new Type[]{typeof(Some_Type_Containg_some_Specs)};
            // var finder = new SpecFinder(types);
            var finder = new SpecFinderWithInheritanceSupport(types);
            var builder = new ContextBuilder(finder, new Tags().Parse(tagOrClassName), new DefaultConventions());

            finder.SpecClasses().ToList().ForEach(x => System.Console.Out.WriteLine($"spec: {x}"));
            var runner = new ContextRunner(builder, new ConsoleFormatter(), false);
            var results = runner.Run(builder.Contexts().Build());

            //assert that there aren't any failures
            results.Failures().Count().should_be(0);
        }
    }

    public class SpecFinderWithInheritanceSupport : ISpecFinder
    {
        private readonly IEnumerable<Type> _types;

        public SpecFinderWithInheritanceSupport(IEnumerable<Type> types)
        {
            _types = types;
        }

        public virtual IEnumerable<Type> SpecClasses()
        {
            return _types.Where(typeof (nspec).IsAssignableFrom);
        }
    }
}
