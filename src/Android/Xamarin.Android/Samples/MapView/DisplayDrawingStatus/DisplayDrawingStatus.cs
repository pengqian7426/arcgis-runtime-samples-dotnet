// Copyright 2016 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Android.App;
using Android.OS;
using Android.Widget;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using Android.Views;

namespace ArcGISRuntime.Samples.DisplayDrawingStatus
{
    [Activity (ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Display drawing status",
        "MapView",
        "This sample demonstrates how to use the DrawStatus value of the MapView to notify user that the MapView is drawing.",
        "")]
    public class DisplayDrawingStatus : Activity
    {
        // Hold a reference to the map view
        private MapView _myMapView;

        // Waiting popup
        private AlertDialog _progressDialog;

        // Status label
        private TextView _statusLabel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "Display drawing status";

            // Create the UI, setup the control references and execute initialization 
            CreateLayout();
            Initialize();
        }

        private void Initialize()
        {
            // Hook up the DrawStatusChanged event
            _myMapView.DrawStatusChanged += OnDrawStatusChanged;

            // Create new Map with basemap
            Map myMap = new Map(BasemapType.Topographic, 34.056, -117.196, 4);

            // Create uri to the used feature service
            Uri serviceUri = new Uri(
                "https://sampleserver6.arcgisonline.com/arcgis/rest/services/DamageAssessment/FeatureServer/0");

            // Initialize a new feature layer
            ServiceFeatureTable myFeatureTable = new ServiceFeatureTable(serviceUri);
            FeatureLayer myFeatureLayer = new FeatureLayer(myFeatureTable);

            // Add the feature layer to the Map
            myMap.OperationalLayers.Add(myFeatureLayer);

            // Provide used Map to the MapView
            _myMapView.Map = myMap;
        }

        private void OnDrawStatusChanged(object sender, DrawStatusChangedEventArgs e)
        {
            // Make sure that the UI changes are done in the UI thread
            RunOnUiThread(() =>
            {
                // Show the activity indicator if the map is drawing
                if (e.Status == DrawStatus.InProgress)
                {
                    _progressDialog.Show();
                    _statusLabel.Text = "Drawing status: In progress";
                }
                else
                {
                    _progressDialog.Hide();
                    _statusLabel.Text = "Drawing status: Complete";
                }
            });
        }

        private void CreateLayout()
        {
            // Create a new vertical layout for the app
            LinearLayout layout = new LinearLayout(this) { Orientation = Orientation.Vertical };

            _statusLabel = new TextView(this);
            _statusLabel.Text = "Drawing status: unknown";

            // Add the views to the layout
            layout.AddView(_statusLabel);
            _myMapView = new MapView(this);
            layout.AddView(_myMapView);

            // Create an activity indicator
            // Show the waiting dialog.
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetView(new ProgressBar(this)
            {
                Indeterminate = true
            });
            builder.SetMessage("Drawing in progress.");
            _progressDialog = builder.Create();

            // Show the layout in the app
            SetContentView(layout);
        }
    }
}