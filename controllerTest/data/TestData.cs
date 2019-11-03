using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using EquipmentControlSystem.Controller;
using Xunit;
using Xunit.Sdk;

namespace EquipmentControlSystem.ControllerTest {
    class TestData : DataAttribute {
        public override IEnumerable<object[]> GetData (MethodInfo testMethod) {
            yield return new object[] {
                1,
                ReadControllerConfig ("../../../data/testDataSet1/ControllerConfig.json"),
                ReadCommands ("../../../data/testDataSet1/Commands.json")
            };
            yield return new object[] {
                2,
                ReadControllerConfig ("../../../data/testDataSet2/ControllerConfig.json"),
                ReadCommands ("../../../data/testDataSet2/Commands.json")
            };
            yield return new object[] {
                3,
                ReadControllerConfig ("../../../data/testDataSet3/ControllerConfig.json"),
                ReadCommands ("../../../data/testDataSet3/Commands.json")
            };
        }

        private ControllerConfig ReadControllerConfig (string filePath) {
            string configJson = ReadFile (filePath);
            ControllerConfig config = ReadConfigFromJsonString (configJson);
            return config;
        }

        private List<string> ReadCommands (string filePath) {
            string commandsJson = ReadFile (filePath);
            List<string> commands = ReadCommandsFromJson (commandsJson);
            return commands;
        }

        private string ReadFile (string path) {
            try {
                var sr = new StreamReader (path);

                return sr.ReadToEnd ();
            } catch (Exception e) {
                EquipmentControllerTest.log ("Exception: " + e.Message);
                return string.Empty;
            }
        }

        private ControllerConfig ReadConfigFromJsonString (string configJson) {
            try {
                var options = new JsonSerializerOptions {
                    AllowTrailingCommas = true
                };

                return JsonSerializer.Deserialize<ControllerConfig> (configJson, options);
            } catch (Exception e) {
                EquipmentControllerTest.log ("Exception: " + e.Message);
                return null;
            }
        }

        private List<string> ReadCommandsFromJson (string commandsJson) {
            try {
                var options = new JsonSerializerOptions {
                    AllowTrailingCommas = true
                };

                return JsonSerializer.Deserialize<List<string>> (commandsJson, options);
            } catch (Exception e) {
                EquipmentControllerTest.log ("Exception: " + e.Message);
                return new List<string> ();
            }
        }
    }
}