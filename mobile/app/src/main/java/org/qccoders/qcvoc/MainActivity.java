/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

package org.qccoders.qcvoc;

import android.Manifest;
import android.app.Activity;
import android.content.ContentResolver;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.Matrix;
import android.net.Uri;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.annotation.NonNull;
import android.support.design.widget.Snackbar;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.FileProvider;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Base64;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.webkit.WebResourceRequest;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;
import android.widget.Toast;

import java.io.ByteArrayOutputStream;
import java.io.File;

import static org.qccoders.qcvoc.Constants.BARCODE_REQUEST;
import static org.qccoders.qcvoc.Constants.prodUrl;

public class MainActivity extends AppCompatActivity {
    private WebView webview;
    private ProgressBar progressBar;
    private String callback;
    private Uri mImageUri;

    private BarcodeHandler barcodeHandler = new BarcodeHandler(this);

    private static final int PHOTO_REQUEST = 1888;
    private static final int PHOTO_PERMISSION_CODE = 100;

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
                    callback = query.substring(query.indexOf('=') + 1);

                    if (queryCommand.equals("scan")) {
                        barcodeHandler.scanBarcode(callback);
                        return true;
                    } else if (queryCommand.equals("acquirePhoto")) {
                        takePhoto();
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

    public void takePhoto() {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED
                || ActivityCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA,
                            Manifest.permission.WRITE_EXTERNAL_STORAGE},
                            PHOTO_PERMISSION_CODE);
        } else {
            captureImage();
        }
    }

    private void captureImage() {
        Intent cameraIntent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
        File photo;
        try
        {
            // place where to store camera taken picture
            File photoDir = getExternalFilesDir(Environment.DIRECTORY_PICTURES);
            photo = File.createTempFile("photo", ".jpg", photoDir);
            photo.delete();
        }
        catch(Exception e)
        {
            Log.v("MainActivity", "Error creating temporary file to store photo", e);
            Toast.makeText(this, "Error creating temporary file to store photo", Toast.LENGTH_LONG).show();
            return;
        }

        mImageUri = FileProvider.getUriForFile(this,
                "org.qccoders.qcvoc.fileprovider",
                photo);
        cameraIntent.putExtra(MediaStore.EXTRA_OUTPUT, mImageUri);

        startActivityForResult(cameraIntent, PHOTO_REQUEST);
    }

    private Bitmap retrievePhoto()
    {
        ContentResolver cr = this.getContentResolver();
        Bitmap bitmap;
        try
        {
            bitmap = android.provider.MediaStore.Images.Media.getBitmap(cr, mImageUri);
            return transformPhoto(bitmap);
        }
        catch (Exception e)
        {
            Toast.makeText(this, "Error retrieving photo", Toast.LENGTH_LONG).show();
            Log.d("MainActivity", "Error retrieving photo", e);
            return Bitmap.createBitmap(300, 300, Bitmap.Config.ARGB_8888);
        }
    }

    private Bitmap transformPhoto(Bitmap bitmap) {
        // Crop to square
        if (bitmap.getWidth() >= bitmap.getHeight()){
            bitmap = Bitmap.createBitmap(
                    bitmap,
                    bitmap.getWidth()/2 - bitmap.getHeight()/2,
                    0,
                    bitmap.getHeight(),
                    bitmap.getHeight()
            );
        } else {
            bitmap= Bitmap.createBitmap(
                    bitmap,
                    0,
                    bitmap.getHeight()/2 - bitmap.getWidth()/2,
                    bitmap.getWidth(),
                    bitmap.getWidth()
            );
        }
        // Scale down
        bitmap = Bitmap.createScaledBitmap(bitmap, 300, 300, false);
        // Rotate
        Matrix matrix = new Matrix();
        matrix.postRotate(90);
        return Bitmap.createBitmap(bitmap, 0, 0, bitmap.getWidth(), bitmap.getHeight(), matrix, true);

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
            captureImage();
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == BARCODE_REQUEST) {
            String returnJavascript = barcodeHandler.returnBarcode(data);
            if (returnJavascript != null) {
                webview.evaluateJavascript(returnJavascript, null);
            }
        }
        if (requestCode == PHOTO_REQUEST && resultCode == Activity.RESULT_OK) {
            Bitmap bitmap = retrievePhoto();

            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.JPEG, 70, stream);
            byte[] byteArray = stream.toByteArray();
            String photo = "data:image/jpeg;base64," + Base64.encodeToString(byteArray, Base64.NO_WRAP);

            Log.d("MainActivity", "Photo of size " + photo.length() + " bytes taken");
            webview.evaluateJavascript(
                    callback + "('" + photo + "')",
                    null
            );
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
