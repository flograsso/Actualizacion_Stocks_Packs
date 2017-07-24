/*
 * Created by SharpDevelop.
 * User: SoporteSEM
 * Date: 13/06/2017
 * Time: 9:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */




using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System.Net;

namespace Actualizar_Stock_Packs
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class EditorTxt{
		
		List<Pack> listaPacks;
		List<string> listaProductos;
		public Logger logger = new Logger();
		const int RESTA_PACKS = 6;//Valor de margen para restar a la cant de packs
		
		public struct Pack
		{
			public long sku_lamp;
			public int sku_pack;
			public int cant_pack;
			public int stock_pack;
			public int margen_pack;
			
			public Pack (long sku_lamp, int cant_pack, int sku_pack,int margen_pack)
			{
				this.sku_lamp=sku_lamp;
				this.sku_pack=sku_pack;
				this.cant_pack=cant_pack;
				this.stock_pack=0;
				this.margen_pack=margen_pack;
			}
			
			public override string ToString()
			{
				return string.Format("[Pack Sku_lamp={0}, Sku_pack={1}, Cant_pack={2}, Stock_pack={3}, Margen_pack={4}", sku_lamp, sku_pack, cant_pack, stock_pack,margen_pack);
			}

			
		}
		
		
		public EditorTxt()
		{
		}
		
		public void leerPacks(string rutaArchivo)
		{
			listaPacks  = new List<Pack>();
			System.IO.StreamReader reader = new System.IO.StreamReader(@rutaArchivo);
			string line;
			
			while ((line = reader.ReadLine()) != null)
			{
				string[] words = line.Split(';');
				listaPacks.Add(new Pack(Int64.Parse(words[0]),Int32.Parse(words[1]),Int32.Parse(words[2]),Int32.Parse(words[3])));

			}
			reader.Close();
		}
		
		public void chequearExistenciaSKU(string rutaArchivoStocks)
		{
			System.IO.StreamReader reader = new System.IO.StreamReader(@rutaArchivoStocks);
			string text;
			bool ok = true;
			text = reader.ReadToEnd();
			foreach (Pack pack in listaPacks)
			{
				if (!text.Contains(pack.sku_lamp.ToString()))
				{
					logger.logData("SKU de lampara inexistente: "+pack.sku_lamp);
					ok=false;
				}
				
				if (!text.Contains(pack.sku_pack.ToString()))
				{
					logger.logData("SKU de pack inexistente: "+pack.sku_pack);
					ok=false;
				}
				
			}
			if (!ok)
				sendErrorEmail();
			
			reader.Close();
		}
		
		public void leerArchivoStock(string rutaArchivoStocks)
		{
			StreamReader reader = new System.IO.StreamReader(@rutaArchivoStocks);
			string line;
			listaProductos = new List<string>();
			
			while ((line = reader.ReadLine()) != null)
			{
				listaProductos.Add(line); //Agrego todas las lineas a una lista
				
			}
			reader.Close();
			reader=null;
			
		}
		
		public void grabarStocks(string rutaArchivoStocks)
		{

			StreamWriter writer = new StreamWriter(@rutaArchivoStocks);
			//Crea el nuevo archivo en el disco C
			StringBuilder aux = new StringBuilder();
			bool esPack = false;
			
			foreach (string texto in listaProductos)
			{
				esPack = false;	
				foreach (Pack pack in listaPacks)
				{
					
					//Tomo solo los primeros 4 caracteres del sku del pack de lamps
					if (texto.Substring(0,4).Contains(pack.sku_pack.ToString()))
					{
						esPack=true;
						aux.Append(texto); //Cargo la linea al StringBuilder
						aux.Remove(95,10); //Borro todo dsp del precio
						if(pack.stock_pack.ToString().Length == 1)
						{
							aux.Insert(95,"      "+pack.stock_pack+".00");
						}
						else if (pack.stock_pack.ToString().Length == 2)
						{
							aux.Insert(95,"     "+pack.stock_pack+".00");
						}
						else if (pack.stock_pack.ToString().Length == 3)
						{
							aux.Insert(95,"    "+pack.stock_pack+".00");
						}
						else if (pack.stock_pack.ToString().Length == 4)
						{
							aux.Insert(95,"   "+pack.stock_pack+".00");
						}
						writer.WriteLine(aux);
						logger.logData(aux.ToString());
						aux.Clear();
					}

				}
				if (!esPack)
				{
					writer.WriteLine(texto);
				}
				
				
			}
			writer.Close();
		}
		
		//Calcula el stock de cada pack y lo escribe en la lista de packs
		public void calcularStock()
		{
			int stockLamp=0;
			string auxString="";
			int aux = 0;
			Pack packaux;
			foreach (string linea in listaProductos)
			{
				for (int x = 0; x < listaPacks.Count; x++)
				{
					if (linea.Contains(listaPacks[x].sku_lamp.ToString()))
					{
						aux=95;
						auxString="";
						while (linea[aux]==' ')
						{
							aux++;
						}
						
						while (linea[aux]!='.')
						{
							auxString=auxString+linea[aux];
							aux++;
						}
						
						
						
						stockLamp=Int32.Parse(auxString);
						
						packaux = new Pack(listaPacks[x].sku_lamp,listaPacks[x].cant_pack,listaPacks[x].sku_pack,listaPacks[x].margen_pack);
						packaux.stock_pack=(int)(Math.Truncate((decimal)stockLamp/listaPacks[x].cant_pack)-listaPacks[x].margen_pack);
						if (packaux.stock_pack<0)
						{
							packaux.stock_pack=0;
						}
						listaPacks[x]=packaux;
						
						
						
						

					}
					

					
					
					
				}
				
			}
		}
		
		public void sendErrorEmail()
		{
			try
			{
				var post = new NameValueCollection();
				post.Add("devid", "v56C2E1A54CA93CC");

				using (var wc = new WebClient())
				{
					wc.UploadValues("http://api.pushingbox.com/pushingbox", post);
				}
				
				post = null;
			}
			catch (WebException e)
			{
				logger.logData("EXEPCION: "+e);
			}
		}
	}
}
