package com.yymoon.snowball;

import java.util.concurrent.*;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.RandomAccessFile;
import java.net.Socket;
import java.net.UnknownHostException;
import java.security.KeyManagementException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.UnrecoverableKeyException;
import java.security.cert.X509Certificate;
import java.util.ArrayList;
import java.util.List;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.HttpVersion;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.ResponseHandler;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.conn.ClientConnectionManager;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.HttpParams;
import org.apache.http.params.HttpProtocolParams;
import org.apache.http.protocol.HTTP;
import org.apache.http.util.EntityUtils;
import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;
import android.provider.ContactsContract;
import android.util.Log;
import static com.alipay.sdk.app.statistic.c.i;
import static com.alipay.sdk.app.statistic.c.s;

public class Util {

	private static final String TAG = "unity";

	public static String httpPost(final String url,final String entity) {
		if (url == null || url.length() == 0) {
			Log.e(TAG, "httpPost, url is null");
			return null;
		}
		ExecutorService exec = Executors.newCachedThreadPool();
		Future<String> f_results;
		f_results=exec.submit(new MyCallable(url,entity));
		exec.shutdown();
		try {
			boolean isFinish = exec.awaitTermination(30, TimeUnit.SECONDS);

			if (!isFinish)
			{
				exec.shutdownNow();
			}
			return  f_results.get();

		} catch (Exception e) {
			e.printStackTrace();
			return"";
		}
	}

	private static class SSLSocketFactoryEx extends SSLSocketFactory {

		SSLContext sslContext = SSLContext.getInstance("TLS");

		public SSLSocketFactoryEx(KeyStore truststore) throws NoSuchAlgorithmException, KeyManagementException, KeyStoreException, UnrecoverableKeyException {
			super(truststore);

			TrustManager tm = new X509TrustManager() {

				public X509Certificate[] getAcceptedIssuers() {
					return null;
				}

				@Override
				public void checkClientTrusted(X509Certificate[] chain, String authType) throws java.security.cert.CertificateException {
				}

				@Override
				public void checkServerTrusted(X509Certificate[] chain,	String authType) throws java.security.cert.CertificateException {
				}
			};
			sslContext.init(null, new TrustManager[] { tm }, null);
		}

		@Override
		public Socket createSocket(Socket socket, String host, int port, boolean autoClose) throws IOException, UnknownHostException {
			return sslContext.getSocketFactory().createSocket(socket, host,	port, autoClose);
		}

		@Override
		public Socket createSocket() throws IOException {
			return sslContext.getSocketFactory().createSocket();
		}
	}

	private static HttpClient getNewHttpClient() {
		try {
			KeyStore trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
			trustStore.load(null, null);

			SSLSocketFactory sf = new SSLSocketFactoryEx(trustStore);
			sf.setHostnameVerifier(SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);

			HttpParams params = new BasicHttpParams();
			HttpProtocolParams.setVersion(params, HttpVersion.HTTP_1_1);
			HttpProtocolParams.setContentCharset(params, HTTP.UTF_8);

			SchemeRegistry registry = new SchemeRegistry();
			registry.register(new Scheme("http", PlainSocketFactory.getSocketFactory(), 80));
			registry.register(new Scheme("https", sf, 443));

			ClientConnectionManager ccm = new ThreadSafeClientConnManager(params, registry);

			return new DefaultHttpClient(ccm, params);
		} catch (Exception e) {
			Log.d(TAG,"getNewHttpClient");
			return new DefaultHttpClient();
		}
	}

	static class MyCallable implements Callable<String>
	{
		private String m_url, m_entity;
		private String res="";
		MyCallable(final String url,final String entity)
		{
			m_url=url;
			m_entity=entity;
		}
		@Override
		public String call() throws Exception
		{
			try {
				HttpClient httpClient = getNewHttpClient();
				HttpPost httpPost = new HttpPost(m_url);
				httpPost.setEntity(new StringEntity(m_entity));
				httpPost.setHeader("Accept", "application/json");
				httpPost.setHeader("Content-type", "application/json");
				ResponseHandler<String> responseHandler = new ResponseHandler<String>() {
					@Override
					public String handleResponse(final HttpResponse response)
							throws ClientProtocolException, IOException {
						int status = response.getStatusLine().getStatusCode();
						if (status == HttpStatus.SC_OK) {
							HttpEntity entity = response.getEntity();
							return  EntityUtils.toString(entity, "UTF-8");
						} else {
							throw new ClientProtocolException(
									"Unexpected response status: " + status);
						}
					}
				};
				HttpResponse result = httpClient.execute(httpPost);
				if (result.getStatusLine().getStatusCode() != HttpStatus.SC_OK) {
					Log.e(TAG, "httpGet fail, status code = " + result.getStatusLine().getStatusCode());
					return "";
				}
				Log.d(TAG, "result: "+result);
				Log.d(TAG, "resultString: "+result.toString());
				res= EntityUtils.toString(result.getEntity(), "UTF-8");
			} catch (Exception e) {
				Log.e(TAG, "httpPost exception!!!, e = " + e.toString());
				e.printStackTrace();
				return "";
			}
			return res;
		}
	}
}


