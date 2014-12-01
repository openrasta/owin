using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Owin;

namespace Tests
{
    public class namespaces_are_correct
    {
        [Test]
        public void for_conversions()
        {
            Assert.That(typeof (MakeBuildFuncUseful).Namespace, Is.EqualTo("Owin"));
        }

        [Test]
        public void for_katana()
        {
            Assert.That(typeof(MakeKatanaUnderstandOwin).Namespace, Is.EqualTo("Owin"));
        }
    }
}
