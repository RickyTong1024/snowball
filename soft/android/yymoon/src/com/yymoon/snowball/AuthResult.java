package com.yymoon.snowball;

import android.text.TextUtils;
import java.util.Map;

public class AuthResult
{
    private String resultStatus;
    private String result;
    private String memo;
    private String resultCode;
    private String authCode;
    private String alipayOpenId;

    public AuthResult(Map<String, String> rawResult, boolean removeBrackets)
    {
        if (rawResult == null) {
            return;
        }
        for (String key : rawResult.keySet()) {
            if (TextUtils.equals(key, "resultStatus")) {
                this.resultStatus = ((String)rawResult.get(key));
            } else if (TextUtils.equals(key, "result")) {
                this.result = ((String)rawResult.get(key));
            } else if (TextUtils.equals(key, "memo")) {
                this.memo = ((String)rawResult.get(key));
            }
        }
        String[] resultValue = this.result.split("&");
        for (String value : resultValue) {
            if (value.startsWith("alipay_open_id")) {
                this.alipayOpenId = removeBrackets(getValue("alipay_open_id=", value), removeBrackets);
            } else if (value.startsWith("auth_code")) {
                this.authCode = removeBrackets(getValue("auth_code=", value), removeBrackets);
            } else if (value.startsWith("result_code")) {
                this.resultCode = removeBrackets(getValue("result_code=", value), removeBrackets);
            }
        }
    }

    private String removeBrackets(String str, boolean remove)
    {
        if ((remove) &&
                (!TextUtils.isEmpty(str)))
        {
            if (str.startsWith("\"")) {
                str = str.replaceFirst("\"", "");
            }
            if (str.endsWith("\"")) {
                str = str.substring(0, str.length() - 1);
            }
        }
        return str;
    }

    public String toString()
    {
        return "resultStatus={" + this.resultStatus + "};memo={" + this.memo + "};result={" + this.result + "}";
    }

    private String getValue(String header, String data)
    {
        return data.substring(header.length(), data.length());
    }

    public String getResultStatus()
    {
        return this.resultStatus;
    }

    public String getMemo()
    {
        return this.memo;
    }

    public String getResult()
    {
        return this.result;
    }

    public String getResultCode()
    {
        return this.resultCode;
    }

    public String getAuthCode()
    {
        return this.authCode;
    }

    public String getAlipayOpenId()
    {
        return this.alipayOpenId;
    }
}