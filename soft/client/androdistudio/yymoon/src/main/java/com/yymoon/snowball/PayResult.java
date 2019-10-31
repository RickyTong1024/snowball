package com.yymoon.snowball;

import android.text.TextUtils;

import java.util.Map;

public class PayResult {
    private String resultStatus;
    private String result;
    private String memo;

    public PayResult(String rawResult)
    {
        if (TextUtils.isEmpty(rawResult)) {
            return;
        }
        String[] resultParams = rawResult.split(";");
        for (String resultParam : resultParams)
        {
            if (resultParam.startsWith("resultStatus")) {
                this.resultStatus = resultParam;
            }
            if (resultParam.startsWith("result")) {
                this.result = resultParam;
            }
            if (resultParam.startsWith("memo")) {
                this.memo = resultParam;
            }
        }
    }

    public String toString() {
        return this.resultStatus+ ";" + this.memo + ";" + this.result;
    }

    public String getResultStatus() {
        return this.resultStatus;
    }

    public String getMemo() {
        return this.memo;
    }

    public String getResult() {
        return this.result;
    }
}
