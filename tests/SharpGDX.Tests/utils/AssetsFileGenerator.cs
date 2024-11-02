//using SharpGDX.Files;
//
//namespace SharpGDX.Tests.Utils;
//
///** Used to generate an assets.txt file for a specific directory.
// * @author mzechner */
//public class AssetsFileGenerator {
//	public static void main (String[] args) {
//		FileHandle file = new FileHandle(args[0]);
//		StringBuffer list = new StringBuffer();
//		args[0] = args[0].replace("\\", "/");
//		if (!args[0].endsWith("/")) args[0] = args[0] + "/";
//		traverse(file, args[0], list);
//		new FileHandle(args[0] + "/assets.txt").writeString(list.toString(), false);
//	}
//
//	private static final void traverse (FileHandle directory, String base, StringBuffer list) {
//		if (directory.name().equals(".svn")) return;
//		String dirName = directory.toString().replace("\\", "/").replace(base, "") + "/";
//		Console.WriteLine(dirName);
//		for (FileHandle file : directory.list()) {
//			if (file.isDirectory()) {
//				traverse(file, base, list);
//			} else {
//				String fileName = file.toString().replace("\\", "/").replace(base, "");
//				if (fileName.endsWith(".png") || fileName.endsWith(".jpg") || fileName.endsWith(".jpeg")) {
//					list.append("i:" + fileName + "\n");
//					Console.WriteLine(fileName);
//				} else if (fileName.endsWith(".glsl") || fileName.endsWith(".fnt") || fileName.endsWith(".pack")
//					|| fileName.endsWith(".obj") || file.extension().equals("") || fileName.endsWith("txt")) {
//					list.append("t:" + fileName + "\n");
//					Console.WriteLine(fileName);
//				} else {
//					if (fileName.endsWith(".mp3") || fileName.endsWith(".ogg") || fileName.endsWith(".wav")) continue;
//					list.append("b:" + fileName + "\n");
//					Console.WriteLine(fileName);
//				}
//			}
//		}
//	}
//}
