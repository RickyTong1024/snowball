using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Ianalytics{
    void onPurchase(string orderId, double currencyAmount, double virtualCurrencyAmount);

    void onGameLogin(string login_param,int is_new);

    void onGameUserUpgrade(int level);
}
