﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FactoryPanel : DragPanel//for dragging
{

    //Tooltip
    public Button upgradeButton, reopenButton, destroyButton, buyButton, sellButton, nationalizeButton;
    public Toggle subidize, dontHireOnSubsidies;
    public Slider priority;
    public GameObject panel;

    public Text generaltext;

    private Factory shownFactory;
    reopenButtonStatus reopenButtonflag;
    // Use this for initialization
    void Start()
    {
        MainCamera.factoryPanel = this;
        Hide();
    }
    //void OnGUI()
    //{
    //    GUI.Button(new Rect(10, 10, 100, 20), new GUIContent("Click me", "This is the tooltip"));
    //    GUI.Label(new Rect(10, 40, 100, 40), GUI.tooltip);
    //}
    enum reopenButtonStatus { reopen, close };
    // Update is called once per frame
    void Update()
    {
        //refresh();
    }
    void setButtonTooltip(Button but, string hint)
    {
        if (hint == null)
        {
            but.interactable = true;
            but.GetComponentInChildren<ToolTipHandler>().tooltip = "";
        }
        else
        {
            but.interactable = false;
            but.GetComponentInChildren<ToolTipHandler>().tooltip = hint;
        }
    }
    void setGUIElementsAccesability()
    {
        var economy = shownFactory.province.owner.economy;

        //upgradeButton.interactable = economy.allowsFactoryUpgradeByGovernment();
        //setButtonTooltip(upgradeButton, shownFactory.whyCantUpgradeFactory(Game.player));
        //upgradeButton.interactable = shownFactory.getConditionsForFactoryUpgrade(Game.player,  out upgradeButton.GetComponentInChildren<ToolTipHandler>().tooltip);
        upgradeButton.interactable = shownFactory.conditionsUpgrade.isAllTrue(Game.player,  out upgradeButton.GetComponentInChildren<ToolTipHandler>().tooltip);

        subidize.interactable = shownFactory.getConditionsForFactorySubsidize(Game.player, false, out subidize.GetComponentInChildren<ToolTipHandler>().tooltip);
        if (shownFactory.isWorking())
            reopenButtonflag = reopenButtonStatus.close;
        else
            reopenButtonflag = reopenButtonStatus.reopen;
        if (reopenButtonflag == reopenButtonStatus.close)
        {
            //reopenButton.interactable = economy.allowsFactoryCloseByGovernment();
            reopenButton.GetComponentInChildren<Text>().text = "Close";
            setButtonTooltip(reopenButton, shownFactory.whyCantCloseFactory());
        }
        else
        {
            //reopenButton.interactable = economy.allowsFactoryReopenByGovernment();
            setButtonTooltip(reopenButton, shownFactory.whyCantReopenFactory());
            reopenButton.GetComponentInChildren<Text>().text = "Reopen";
        }

        //destroyButton.interactable = economy.allowsFactoryDestroyByGovernment();
        // hint = shownFactory.whyCantDestroyFactory();
        setButtonTooltip(destroyButton, shownFactory.whyCantDestroyFactory());

        sellButton.interactable = economy.allowsFactorySellByGovernment();
        buyButton.interactable = economy.allowsFactoryBuyByGovernment();
        nationalizeButton.interactable = economy.allowsFactoryNatonalizeByGovernment();

    }
    public void refresh()
    {
        if (shownFactory != null)
        {
            setGUIElementsAccesability();
            //var temp = shownFactory.getLifeNeeds();
            //foreach (Storage next in temp)
            //    lifeNeeds += next.ToString() + "; ";
            //lifeNeeds += shownFactory.getLifeNeedsFullfilling().ToString() + " fullfilled";
            string InputRequired = "";
            //var temp = shownFactory.getLifeNeeds();
            //todo anti-mirorring
            string construction = "";
            if (shownFactory.getDaysInConstruction() > 0)
                construction = "\nDays in construction: " + shownFactory.getDaysInConstruction();
            string unprofitable = "";
            if (shownFactory.getDaysUnprofitable() > 0)
                unprofitable = " Days unprofitable: " + shownFactory.getDaysUnprofitable();
            string daysClosed = "";
            if (shownFactory.getDaysClosed() > 0)
                daysClosed = " Days closed: " + shownFactory.getDaysClosed();
            string loans = "";
            if (shownFactory.loans.get() > 0f)
                loans = "\nLoan: " + shownFactory.loans.ToString();
            string upgradeNeeds = "";
            if (shownFactory.needsToUpgrade.Count() > 0)
                upgradeNeeds = "\nUpgrade needs: " + shownFactory.needsToUpgrade;

            foreach (Storage next in shownFactory.type.resourceInput)
                InputRequired += next.get() * shownFactory.getWorkForceFullFilling() + " " + next.getProduct().ToString() + ";";
            generaltext.text = shownFactory.type.name + " level: " + shownFactory.getLevel() + "\n" + "Workforce: " + shownFactory.getWorkForce()
                + "\nUnsold: " + shownFactory.storageNow.ToString()
                + "\nGain goods: " + shownFactory.gainGoodsThisTurn.ToString()
                + "\nSent to market: " + shownFactory.sentToMarket
                + "\nCash: " + shownFactory.wallet.ToString()
                + "\nMoney income: " + shownFactory.wallet.moneyIncomethisTurn
                + "\nProfit: " + shownFactory.getProfit()
                + "\nInput required: " + InputRequired
                + "\nConsumed: " + shownFactory.consumedTotal.ToString() + " Costed: " + shownFactory.getConsumedCost().ToString()
                + "\nConsumed LT: " + shownFactory.consumedLastTurn
                + "\nSalary (per 1000 men):" + shownFactory.getSalary() + " Salary(total):" + shownFactory.getSalaryCost()
                + "\nOwner: " + shownFactory.factoryOwner.ToString()
                + upgradeNeeds
                + construction + unprofitable + daysClosed + loans
                + "\nHowMuchHiredLastTurn " + shownFactory.getHowMuchHiredLastTurn()
                ;
            //+ "\nExpenses:"
        }
    }
    public void Show(Factory fact)
    {
        panel.SetActive(true);
        shownFactory = fact;
        panelRectTransform.SetAsLastSibling();
        refresh();
    }
    public void removeFactory(Factory fact)
    {
        if (fact == shownFactory)
        {
            shownFactory = null;
            Hide();
        }
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
    public void onCloseClick()
    {
        Hide();
    }
    public void onReopenClick()
    {
        if (reopenButtonflag == reopenButtonStatus.close)
            //if (shownFactory.whyCantCloseFactory() == null)
            shownFactory.close();
        else
            //if (shownFactory.whyCantReopenFactory() == null)
            shownFactory.reopen();
        refresh();
        if (MainCamera.productionWindow.isActiveAndEnabled) MainCamera.productionWindow.refresh();
    }
    public void onUpgradeClick()
    {
        if (shownFactory.getConditionsForFactoryUpgradeFast(Game.player))
        {
            shownFactory.upgrade(Game.player);
            if (MainCamera.productionWindow.isActiveAndEnabled) MainCamera.productionWindow.refresh();
            this.refresh();
        }
    }
    public void onDestroyClick()
    {
        //if (shownFactory.whyCantDestroyFactory() == null)
        {
            shownFactory.destroyImmediately();
            if (MainCamera.buildPanel.isActiveAndEnabled) MainCamera.buildPanel.refresh();
        }
    }
    public void onBuyClick()
    {

    }
    public void onSellClick()
    {

    }
    public void onNationalizeClick()
    {

    }

}