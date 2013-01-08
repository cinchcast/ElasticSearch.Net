using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using ElasticSearch.Client.Exception;
using Exortech.NetReflector;

namespace ElasticSearch.Client.Config
{
	[ReflectorType("ElasticSearchConfig")]
	public class ElasticSearchConfig
	{
		#region static method

		private static ElasticSearchConfig _instance = CreateElasticSearchConfig();

		public static ElasticSearchConfig Instance
		{
			get { return _instance; }
		}

		public static event EventHandler ConfigChanged;

		public static void OnConfigChanged()
		{
			if (ConfigChanged != null)
				ConfigChanged(Instance, EventArgs.Empty);
		}

		#endregion

		public ElasticSearchConfig()
		{
			ConnectionPool = new ConnectionPoolConfig();
		}

		[ReflectorCollection("Clusters", InstanceType = typeof(ClusterDefinition[]), Required = true)]
		public ClusterDefinition[] Clusters { get; set; }

		[ReflectorCollection("ConnectionPool", InstanceType = typeof(ConnectionPoolConfig), Required = false)]
		public ConnectionPoolConfig ConnectionPool { set; get; }

        private static ElasticSearchConfig CreateElasticSearchConfig()
		{
			string configName = "Config/ElasticSearch.config";
			string configFileValue = ConfigurationManager.AppSettings["ElasticSearchConfigFile"];

			if (!string.IsNullOrEmpty(configFileValue))
                configName = configFileValue;

            return LoadConfig<ElasticSearchConfig>(configName);
		}

		private static T LoadConfig<T>(string xmlPath) where T : class
		{
			try
			{
				using (var xml = new XmlTextReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlPath)))
				{
					var config = NetReflector.Read(xml) as T;
					xml.Close();
					return config;
				}
			}
			catch (System.Exception exp)
			{
				throw new ElasticSearchException("Failed on loading config !", exp);
			}
		}
		
	}
}