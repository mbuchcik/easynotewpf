using System;
using System.Linq;
using Autofac;
using EasyNote.Integration.EasyNoteAPI.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyNote.Integration.EasyNoteAPI.Tests
{
    [TestClass]
    public class APITests
    {
        private readonly IEasyNoteService service;

        public APITests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new IoC.Module());
            var container = builder.Build();
            this.service = container.Resolve<IEasyNoteService>();

            service.Login(
               new UserInfo
               {
                   Email = "test@test.com",
                   Password = "12345"
               });
        }

        [TestMethod]
        public void should_add_note_not_throw_exception()
        {
            var newNote = new CreateFileCommand
            {
                Name = $"Test file {DateTime.Now.Ticks}",
                Author = "ApiTester",
                Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
            };

            bool exceptionThrown = false;
            try
            {
                service.Add(newNote);
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsFalse(exceptionThrown);
        }

        [TestMethod]
        public void should_get_list_of_files_from_api()
        {
            var list = service.Get();

            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void should_get_single_note_from_api()
        {
            var allNotes = service.Get();
            var note = service.Get(allNotes.First().Id);

            Assert.IsNotNull(note);
        }

        [TestMethod]
        public void should_update_note()
        {
            var allNotes = service.Get();
            var note = allNotes.First();

            var command = new UpdateFileCommand
            {
                Id = note.Id,
                Author = note.Author,
                Content = note.Content + Environment.NewLine + DateTime.Now.Ticks,
                Name = note.Name
            };

            service.Update(command);

            var updatedNote = service.Get(note.Id);

            Assert.AreEqual(command.Content, updatedNote.Content);
        }

        [TestMethod]
        public void should_delete_last_note()
        {
            var allNotes = service.Get();
            var lastNote = allNotes.Last();

            service.Delete(lastNote.Id);

            var allNotesAfterDeletion = service.Get();

            Assert.IsTrue(!allNotesAfterDeletion.Any(x => x.Id == lastNote.Id));
        }
    }
}
