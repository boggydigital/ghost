using System.IO;
using Interfaces.Delegates.Move;

namespace Delegates.Move.IO
{
    public class MoveDirectoryDelegate : IMoveDelegate<string>
    {
        public void Move(string fromUri, string toUri)
        {
            var destination = Path.Combine(toUri, fromUri);
            var destinationDirectory = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);
            Directory.Move(
                fromUri,
                destination);
        }
    }
}