/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

package org.qccoders.qcvoc;

import android.Manifest;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.support.annotation.NonNull;
import android.support.design.widget.Snackbar;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.webkit.WebResourceRequest;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;
import android.widget.Toast;

import static org.qccoders.qcvoc.Constants.BARCODE_REQUEST;
import static org.qccoders.qcvoc.Constants.PHOTO_PERMISSION_CODE;
import static org.qccoders.qcvoc.Constants.PHOTO_REQUEST;
import static org.qccoders.qcvoc.Constants.prodUrl;

public class MainActivity extends AppCompatActivity {
    private WebView webview;
    private ProgressBar progressBar;

    private BarcodeHandler barcodeHandler = new BarcodeHandler(this);
    private PhotoHandler photoHandler = new PhotoHandler(this);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        progressBar = findViewById(R.id.loading_spinner);
        progressBar.setVisibility(View.GONE);

        webview = findViewById(R.id.webView);
        webview.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                super.onPageStarted(view, url, favicon);
                progressBar.setVisibility(View.VISIBLE);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);
                progressBar.setVisibility(View.GONE);
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                if (!Utilities.urlIsTrusted(view.getUrl())) {
                    return true;
                }

                String query = request.getUrl().getQuery();

                if (query != null) {
                    String queryCommand = query.substring(0, query.indexOf('&'));
                    String callback = query.substring(query.indexOf('=') + 1);

                    if (queryCommand.equals("scan")) {
                        barcodeHandler.scanBarcode(callback);
                        return true;
                    } else if (queryCommand.equals("acquirePhoto")) {
                        takePhotoWithPermission(callback);
                        return true;
                    }
                }

                return !Utilities.urlIsTrusted(request.getUrl().toString());
            }
        });

        webview.getSettings().setJavaScriptEnabled(true);
        webview.getSettings().setDomStorageEnabled(true);
        webview.loadUrl(prodUrl);
    }

    public void takePhotoWithPermission(String callback) {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED
                || ActivityCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED){
            photoHandler.setCallback(callback);
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA,
                            Manifest.permission.WRITE_EXTERNAL_STORAGE},
                            PHOTO_PERMISSION_CODE);
        } else {
            photoHandler.capturePhotoWithCallback(callback);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == PHOTO_PERMISSION_CODE) {
            for (int grantResult : grantResults) {
                if (grantResult != PackageManager.PERMISSION_GRANTED) {
                    Toast.makeText(this, "Required permissions were not granted for photo taking", Toast.LENGTH_LONG).show();
                    return;
                }
            }
            photoHandler.capturePhoto();
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
       if (requestCode == BARCODE_REQUEST && resultCode == Activity.RESULT_OK) {
            webview.evaluateJavascript(barcodeHandler.returnBarcode(data), null);
        } else if (requestCode == PHOTO_REQUEST && resultCode == Activity.RESULT_OK) {
            webview.evaluateJavascript(photoHandler.returnPhoto(), null);
        }
    }

    @Override
    public boolean onKeyUp(int keycode, KeyEvent event) {
        String newUrl = CodeHandler.inputKey(keycode);

        if (newUrl != null) {
            String message = "Environment switched to " + newUrl;

            Log.d("MainActivity", message);

            Snackbar.make(
                    findViewById(R.id.mainLayout),
                    message,
                    Snackbar.LENGTH_LONG)
                    .show();

            webview.loadUrl(newUrl);
        }

        return super.onKeyUp(keycode, event);
    }
}
