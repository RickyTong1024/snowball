package com.yymoon.snowball;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.Collator;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Random;

public class OrderInfoUtil2_0
{
    public static Map<String, String> buildAuthInfoMap(String pid, String app_id, String target_id)
    {
        Map<String, String> keyValues = new HashMap();


        keyValues.put("app_id", app_id);


        keyValues.put("pid", pid);


        keyValues.put("apiname", "com.alipay.account.auth");


        keyValues.put("app_name", "mc");


        keyValues.put("biz_type", "openservice");



        keyValues.put("product_id", "APP_FAST_LOGIN");


        keyValues.put("scope", "kuaijie");


        keyValues.put("target_id", target_id);


        keyValues.put("auth_type", "AUTHACCOUNT");


        keyValues.put("sign_type", "RSA");

        return keyValues;
    }

    public static Map<String, String> buildOrderParamMap(String app_id, String body, String subject, String price)
    {
        Map<String, String> keyValues = new HashMap();
        SimpleDateFormat sDateFormat = new SimpleDateFormat("yyyy-MM-dd hh:mm:ss");
        String date = sDateFormat.format(new Date());

        keyValues.put("app_id", app_id);

        keyValues.put(
                "biz_content", "{\"timeout_express\":\"30m\",\"product_code\":\"QUICK_MSECURITY_PAY\",\"total_amount\":\"" + price + "\",\"subject\":\"" + subject + "\",\"body\":\"" +
                        body + "\",\"out_trade_no\":\"" + getOutTradeNo() + "\"}");

        keyValues.put("charset", "utf-8");

        keyValues.put("method", "alipay.trade.app.pay");

        keyValues.put("sign_type", "RSA");


        keyValues.put("timestamp", date);

        keyValues.put("version", "1.0");

        keyValues.put("notify_url", "http://121.43.107.164:8080/notifyapp");

        return keyValues;
    }

    public static String buildOrderParam(Map<String, String> map, boolean isEncode)
    {
        List<String> keys = new ArrayList(map.keySet());
        Collections.sort(keys, Collator.getInstance(Locale.CHINA));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < keys.size() - 1; i++)
        {
            String key = (String)keys.get(i);
            String value = (String)map.get(key);
            sb.append(buildKeyValue(key, value, isEncode));
            sb.append("&");
        }
        String tailKey = (String)keys.get(keys.size() - 1);
        String tailValue = (String)map.get(tailKey);
        sb.append(buildKeyValue(tailKey, tailValue, isEncode));

        return sb.toString();
    }

    private static String buildKeyValue(String key, String value, boolean isEncode)
    {
        StringBuilder sb = new StringBuilder();
        sb.append(key);
        sb.append("=");
        if (isEncode) {
            try
            {
                sb.append(URLEncoder.encode(value, "UTF-8"));
            }
            catch (UnsupportedEncodingException e)
            {
                sb.append(value);
            }
        } else {
            sb.append(value);
        }
        return sb.toString();
    }

    public static String getSign(Map<String, String> map, String rsaKey)
    {
        return "";
    }

    private static String getOutTradeNo()
    {
        SimpleDateFormat format = new SimpleDateFormat("MMddHHmmss", Locale.getDefault());
        Date date = new Date();
        String key = format.format(date);

        Random r = new Random();
        key = key + r.nextInt();
        key = key.substring(0, 15);
        return key;
    }
}
