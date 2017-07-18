/*
 * Created by SharpDevelop.
 * User: usuario
 * Date: 16/01/2015
 * Time: 12:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;




/*
 *	Desde Settings1 se puede cambiar la ubicacion de los archivos txt. Luego de modificarla, hay que ir al settings1.designer y actualizar
 *  el namespace 
 */
namespace Actualizar_Stock_Packs
{
	class Program
	{
		
		
		public static void Main(string[] args)
		{
			string rutaArchivoPacks = Settings1.Default.rutaArchivoPacks;;
			
			string rutaArchivoStock = Settings1.Default.rutaArchivoStock;
			
			EditorTxt editorTxt = new EditorTxt();
			
			editorTxt.logger.logData("COMENZANDO LA EJECUCION");
			try {
				editorTxt.leerPacks(rutaArchivoPacks);
				editorTxt.leerArchivoStock(rutaArchivoStock);
				editorTxt.chequearExistenciaSKU(rutaArchivoStock);
				editorTxt.calcularStock();
				editorTxt.grabarStocks(rutaArchivoStock);
			}
			catch (Exception e)
			{
				editorTxt.sendErrorEmail();
				editorTxt.logger.logData("ERROR: "+e);
				
			}
			
			
			editorTxt.logger.logData("EJECUCION COMPLETADA CON EXITO");
		}
		
		
	}
}