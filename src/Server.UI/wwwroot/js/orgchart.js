let _depsPromise;

function loadScriptOnce(src) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[data-src="${src}"]`);
        if (existing) {
            if (existing.getAttribute("data-loaded") === "1") return resolve();
            existing.addEventListener("load", () => resolve(), { once: true });
            existing.addEventListener("error", () => reject(new Error(`Failed to load ${src}`)), { once: true });
            return;
        }

        const s = document.createElement("script");
        s.src = src;
        s.async = true;
        s.defer = true;
        s.setAttribute("data-src", src);
        s.onload = () => {
            s.setAttribute("data-loaded", "1");
            resolve();
        };
        s.onerror = () => reject(new Error(`Failed to load ${src}`));
        document.head.appendChild(s);
    });
}

async function ensureOrgChartDeps() {
    if (!_depsPromise) {
        _depsPromise = (async () => {
            // 1) d3 UMD -> window.d3
            await loadScriptOnce("https://unpkg.com/d3@7.9.0/dist/d3.min.js");
            const d3 = window.d3;
            if (!d3) throw new Error("d3 not found on window after loading UMD bundle.");

            // 2) d3-flextree UMD -> patches/exports flextree
            await loadScriptOnce("https://unpkg.com/d3-flextree@2.1.2/build/d3-flextree.min.js");

            if (typeof d3.flextree !== "function") {
                if (typeof window.flextree === "function") {
                    d3.flextree = window.flextree;
                }
            }
            if (typeof d3.flextree !== "function") {
                throw new Error("d3.flextree is not a function after loading d3-flextree.");
            }

            // 3) d3-org-chart UMD
            await loadScriptOnce("https://unpkg.com/d3-org-chart@3.1.1/build/d3-org-chart.min.js");

            const OrgChart =
                window.OrgChart ||
                (window.d3 && window.d3.OrgChart) ||
                (window.d3OrgChart && window.d3OrgChart.OrgChart);

            if (!OrgChart) {
                throw new Error("OrgChart not found on window. Check window.d3.OrgChart / window.OrgChart.");
            }

            return { d3, OrgChart };
        })();
    }

    return _depsPromise;
}

export async function createOrgChart(orgdata) {
    console.log(orgdata)
    const { d3, OrgChart } = await ensureOrgChartDeps();

    let chart;

    const styleVars = {
        cardBg: '#FFFFFF',
        borderColor: '#E2E8F0',
        nameColor: '#1E293B',
        roleColor: '#64748B',
        shadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        highlightRing: '0 0 0 4px rgba(99, 102, 241, 0.4)',
        highlightBorder: '#6366f1',
        highlightScale: 'scale(1.05)'
    };


    const container = document.querySelector('.chart-container');
    if (!container) return;


    const activeNode = orgdata?.find?.(x => x.isLoggedUser === true);

    chart = new OrgChart()
        .nodeHeight(d => 135)
        .nodeWidth(d => 290)
        .childrenMargin(d => 50)
        .compactMarginBetween(d => 35)
        .compactMarginPair(d => 30)
        .neighbourMargin((a, b) => 20)
        .linkUpdate(function () {
            d3.select(this)
                .attr("stroke", "#CBD5E1")
                .attr("stroke-width", 1.5);
        })
        .nodeUpdate(function (d) {
            const card = d3.select(this).select('.org-card-container');
            const isHighlighted = d3.select(this).classed('node-highlighted');

            if (isHighlighted) {
                card.style('box-shadow', styleVars.shadow + ', ' + styleVars.highlightRing)
                    .style('border-color', styleVars.highlightBorder)
                    .style('transform', styleVars.highlightScale)
                    .style('z-index', '999');
            } else {
                const isLogged = d.data.isLoggedUser;
                card.style('box-shadow', styleVars.shadow)
                    .style('border-color', isLogged ? '#3B82F6' : styleVars.borderColor)
                    .style('transform', 'scale(1)')
                    .style('z-index', '1');
            }
        })
        .nodeContent(function (d) {
            const isLogged = d.data.isLoggedUser;
            const borderColor = isLogged ? '#3B82F6' : styleVars.borderColor;
            const borderWidth = isLogged ? '2px' : '1px';
            const barColor = isLogged ? '#3B82F6' : '#6366f1';

            const email = d.data.email || '';
            const phone = d.data.phoneNumber || '';

            return `
        <div style="font-family: 'Inter', system-ui, sans-serif; 
                    width:${d.width}px; 
                    height:${d.height}px;
                    box-sizing: border-box;
                    padding: 10px;">
            <div class="org-card-container" style="
                background-color: ${styleVars.cardBg};
                width: 100%; 
                height: 100%;
                border-radius: 8px;
                border: ${borderWidth} solid ${borderColor};
                box-shadow: ${styleVars.shadow};
                display: flex;
                position: relative;
                overflow: hidden;
                transform-origin: center center; 
                transition: all 0.3s ease;">
                
                <!-- Left side colored bar -->
                <div style="width: 6px; height: 100%; background-color: ${barColor}; flex-shrink: 0;"></div>

                <!-- Right side main content area: changed to Flex Column layout -->
                <div style="flex: 1; display: flex; flex-direction: column; padding: 12px 16px; min-width: 0;">
                    
                    <!-- Top half: avatar and name/position -->
                    <div style="display: flex; align-items: center; margin-bottom: 8px;">
                        <div style="position: relative; margin-right: 14px; flex-shrink: 0;">
                            <img src="${d.data.profileUrl}" 
                                 onerror="this.src='/img/avatar.png'"
                                 style="width: 48px; height: 48px; border-radius: 50%; object-fit: cover; border: 2px solid #F1F5F9;" />
                        </div>
                        <div style="display: flex; flex-direction: column; justify-content: center; overflow: hidden;">
                            <div style="font-size: 16px; font-weight: 600; color: ${styleVars.nameColor}; white-space: nowrap; text-overflow: ellipsis; overflow: hidden; line-height: 1.2;">
                                ${d.data.name}
                            </div>
                            <div style="font-size: 12px; font-weight: 400; color: ${styleVars.roleColor}; margin-top: 4px; white-space: nowrap; text-overflow: ellipsis; overflow: hidden;">
                                ${d.data.positionName}
                            </div>
                        </div>
                    </div>

                    <!-- Divider line (optional, adds visual hierarchy) -->
                    <div style="height: 1px; background-color: #F1F5F9; margin-bottom: 8px; width: 100%;"></div>

                    <!-- Bottom half: contact information (12px font) -->
                    <div style="display: flex; flex-direction: column; justify-content: center; font-size: 12px; color: ${styleVars.roleColor};">
                        <!-- Email row -->
                        <div style="display: flex; align-items: center; margin-bottom: 2px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;" title="${email}">
                           <span style="margin-right: 6px; color: #94A3B8;">✉</span>
                           <span style="overflow: hidden; text-overflow: ellipsis;">${email}</span>
                        </div>
                    </div>

                </div>

                <!-- Top right ID badge -->
                <div style="position: absolute; top: 8px; right: 8px; 
                            background-color: #F1F5F9; color: #94A3B8; 
                            padding: 2px 6px; border-radius: 4px; 
                            font-size: 10px; font-weight: 500;">
                    #${String(d.data.id).slice(-3)}
                </div>
            </div>
        </div>`;
        })
        .container('.chart-container')
        .data(orgdata)
        .render();

    if (activeNode) {
        chart.setHighlighted(activeNode.id).render();
    }

}
